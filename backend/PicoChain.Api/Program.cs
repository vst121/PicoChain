using PicoChain.Infrastructure;
using PicoChain.Application.Shipments;
using PicoChain.Application.Approvals;
using PicoChain.Application.Agents;
using PicoChain.Application.Auditing;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.EntityFrameworkCore;
using PicoChain.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddPolicy("frontend", policy =>
    policy.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["http://localhost:4200"])
        .AllowAnyHeader()
        .AllowAnyMethod()));
builder.Services.AddHttpClient("ai-service", client =>
    client.BaseAddress = new Uri(builder.Configuration["AiService:BaseUrl"] ?? "http://localhost:8000"));
builder.Services.AddSingleton<RealtimeEventHub>();
builder.Services.AddHealthChecks();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("picochain-api"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"] ?? "http://localhost:4317");
        }));

var app = builder.Build();

app.UseCors("frontend");

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PicoChainDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.ApplyAsync(db);
}

app.MapGet("/api/v1/events", async (HttpResponse response, RealtimeEventHub hub, CancellationToken cancellationToken) =>
{
    response.Headers.ContentType = "text/event-stream";
    response.Headers.CacheControl = "no-cache";
    await foreach (var message in hub.ReadAllAsync(cancellationToken))
        await response.WriteAsync(message, cancellationToken);
});

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "picochain-api" }));

app.MapPost("/api/v1/copilot/investigate", async (
    CopilotInvestigationRequest request,
    IHttpClientFactory httpClientFactory,
    IAgentTaskService agentTaskService,
    IAgentExecutionGateway executionGateway,
    CancellationToken cancellationToken) =>
{
    AgentTaskCreatedResult task;
    try
    {
        task = await agentTaskService.CreateShipmentInvestigationAsync(
            request.ShipmentId, request.CorrelationId, null, cancellationToken);
    }
    catch (InvalidOperationException)
    {
        return Results.NotFound();
    }

    using var client = httpClientFactory.CreateClient("ai-service");
    using var response = await client.PostAsJsonAsync(
        "/api/v1/investigations",
        new { shipment_id = request.ShipmentId, correlation_id = request.CorrelationId, agent_task_id = task.AgentTaskId },
        cancellationToken);
    var body = await response.Content.ReadAsStringAsync(cancellationToken);
    if (!response.IsSuccessStatusCode)
        return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);

    var investigation = System.Text.Json.JsonSerializer.Deserialize<AiInvestigationResponse>(body)!;
    var proposal = await executionGateway.SubmitRecommendationAsync(
        new AgentRecommendation(
            task.AgentTaskId,
            "CarrierContact",
            System.Text.Json.JsonSerializer.Serialize(new { investigation.recommended_action }),
            string.Join("; ", investigation.evidence)),
        cancellationToken);
    return Results.Ok(new { investigation, agentTaskId = task.AgentTaskId, proposal });
});
app.MapHealthChecks("/health/ready");

app.MapGet("/api/v1/shipments", async (
    string? status,
    IShipmentQueryService shipmentQueryService,
    CancellationToken cancellationToken) =>
{
    var shipments = await shipmentQueryService.ListAsync(status, cancellationToken);
    return Results.Ok(shipments);
});

app.MapGet("/api/v1/shipments/{id:guid}", async (
    Guid id,
    IShipmentQueryService shipmentQueryService,
    CancellationToken cancellationToken) =>
{
    var shipment = await shipmentQueryService.GetAsync(id, cancellationToken);
    return shipment is null ? Results.NotFound() : Results.Ok(shipment);
});

app.MapPost("/api/v1/shipments/{id:guid}/delay", async (
    Guid id,
    DelayShipmentRequest request,
    HttpContext httpContext,
    IShipmentEventService shipmentEventService,
    RealtimeEventHub eventHub,
    CancellationToken cancellationToken) =>
{
    var correlationId = httpContext.Request.Headers["X-Correlation-Id"].FirstOrDefault()
        ?? Guid.NewGuid().ToString("N");
    var traceParent = httpContext.Request.Headers["traceparent"].FirstOrDefault();
    var updated = await shipmentEventService.MarkDelayedAsync(
        id,
        request.Reason,
        correlationId,
        traceParent,
        cancellationToken);

    if (updated) eventHub.Publish("shipment-delayed", new { shipmentId = id, correlationId });
    return updated ? Results.Accepted() : Results.NotFound();
});

app.MapPost("/api/v1/approvals/{id:guid}/approve", async (
    Guid id,
    IApprovalService approvalService,
    RealtimeEventHub eventHub,
    CancellationToken cancellationToken) =>
{
    var result = await approvalService.ApproveAsync(id, "development-user", cancellationToken);
    if (result is not null) eventHub.Publish("approval-updated", result);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.MapPost("/api/v1/agent-tasks/shipment-investigations", async (
    ShipmentInvestigationRequest request,
    HttpContext httpContext,
    IAgentTaskService agentTaskService,
    CancellationToken cancellationToken) =>
{
    var correlationId = httpContext.Request.Headers["X-Correlation-Id"].FirstOrDefault()
        ?? Guid.NewGuid().ToString("N");
    var traceId = httpContext.TraceIdentifier;

    try
    {
        var result = await agentTaskService.CreateShipmentInvestigationAsync(
            request.ShipmentId,
            correlationId,
            traceId,
            cancellationToken);
        return Results.Accepted($"/api/v1/agent-tasks/{result.AgentTaskId}", result);
    }
    catch (InvalidOperationException)
    {
        return Results.NotFound();
    }
});

app.MapGet("/api/v1/agent-tasks", async (
    IAgentTaskQueryService agentTaskQueryService,
    CancellationToken cancellationToken) =>
{
    var tasks = await agentTaskQueryService.ListAsync(cancellationToken);
    return Results.Ok(tasks);
});

app.MapGet("/api/v1/agent-tasks/{id:guid}", async (
    Guid id,
    IAgentTaskQueryService agentTaskQueryService,
    CancellationToken cancellationToken) =>
{
    var task = await agentTaskQueryService.GetAsync(id, cancellationToken);
    return task is null ? Results.NotFound() : Results.Ok(task);
});

app.MapPost("/api/v1/agent-tasks/{id:guid}/start", async (
    Guid id,
    IAgentTaskLifecycleService lifecycleService,
    CancellationToken cancellationToken) =>
    await TransitionAgentTaskAsync(() => lifecycleService.StartAsync(id, cancellationToken)));

app.MapPost("/api/v1/agent-tasks/{id:guid}/complete", async (
    Guid id,
    IAgentTaskLifecycleService lifecycleService,
    CancellationToken cancellationToken) =>
    await TransitionAgentTaskAsync(() => lifecycleService.CompleteAsync(id, cancellationToken)));

app.MapPost("/api/v1/agent-tasks/{id:guid}/fail", async (
    Guid id,
    AgentTaskFailureRequest request,
    IAgentTaskLifecycleService lifecycleService,
    CancellationToken cancellationToken) =>
    await TransitionAgentTaskAsync(() => lifecycleService.FailAsync(id, request.Reason, cancellationToken)));

app.MapGet("/internal/agent-gateway/shipments/{id:guid}/context", async (
    Guid id,
    IAgentExecutionGateway gateway,
    CancellationToken cancellationToken) =>
{
    var context = await gateway.GetShipmentContextAsync(id, cancellationToken);
    return context is null ? Results.NotFound() : Results.Ok(context);
});

app.MapPost("/internal/agent-gateway/recommendations", async (
    AgentRecommendation recommendation,
    IAgentExecutionGateway gateway,
    CancellationToken cancellationToken) =>
{
    var result = await gateway.SubmitRecommendationAsync(recommendation, cancellationToken);
    return result is null ? Results.NotFound() : Results.Created($"/api/v1/action-proposals/{result.ProposalId}", result);
});

app.MapPost("/api/v1/approvals/{id:guid}/reject", async (
    Guid id,
    IApprovalService approvalService,
    RealtimeEventHub eventHub,
    CancellationToken cancellationToken) =>
{
    var result = await approvalService.RejectAsync(id, "development-user", cancellationToken);
    if (result is not null) eventHub.Publish("approval-updated", result);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.MapGet("/api/v1/approvals/pending", async (
    IApprovalQueryService approvalQueryService,
    CancellationToken cancellationToken) =>
{
    var approvals = await approvalQueryService.ListPendingAsync(cancellationToken);
    return Results.Ok(approvals);
});

app.MapGet("/api/v1/approvals/{id:guid}", async (
    Guid id,
    IApprovalQueryService approvalQueryService,
    CancellationToken cancellationToken) =>
{
    var approval = await approvalQueryService.GetAsync(id, cancellationToken);
    return approval is null ? Results.NotFound() : Results.Ok(approval);
});

app.MapGet("/api/v1/audit", async (
    string? correlationId,
    int? limit,
    IAuditQueryService auditQueryService,
    CancellationToken cancellationToken) =>
{
    var records = await auditQueryService.ListAsync(
        correlationId,
        limit ?? 100,
        cancellationToken);
    return Results.Ok(records);
});

app.Run();

static async Task<IResult> TransitionAgentTaskAsync(Func<Task<bool>> transition)
    => await transition() ? Results.Ok() : Results.NotFound();

public sealed record DelayShipmentRequest(string Reason);
public sealed record ShipmentInvestigationRequest(Guid ShipmentId);
public sealed record AgentTaskFailureRequest(string Reason);
public sealed record CopilotInvestigationRequest(Guid ShipmentId, string CorrelationId);
public sealed record AiInvestigationResponse(
    [property: System.Text.Json.Serialization.JsonPropertyName("shipment_id")] Guid ShipmentId,
    [property: System.Text.Json.Serialization.JsonPropertyName("probable_cause")] string ProbableCause,
    [property: System.Text.Json.Serialization.JsonPropertyName("evidence")] string[] Evidence,
    [property: System.Text.Json.Serialization.JsonPropertyName("recommended_action")] string RecommendedAction,
    [property: System.Text.Json.Serialization.JsonPropertyName("requires_approval")] bool RequiresApproval,
    [property: System.Text.Json.Serialization.JsonPropertyName("confidence")] double Confidence,
    [property: System.Text.Json.Serialization.JsonPropertyName("decision_summary")] string DecisionSummary);

public partial class Program
{
}
