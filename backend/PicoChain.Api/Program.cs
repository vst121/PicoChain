using PicoChain.Infrastructure;
using PicoChain.Application.Shipments;
using PicoChain.Application.Approvals;
using PicoChain.Application.Agents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "picochain-api" }));
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

    return updated ? Results.Accepted() : Results.NotFound();
});

app.MapPost("/api/v1/approvals/{id:guid}/approve", async (
    Guid id,
    IApprovalService approvalService,
    CancellationToken cancellationToken) =>
{
    var result = await approvalService.ApproveAsync(id, "development-user", cancellationToken);
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

app.MapPost("/api/v1/approvals/{id:guid}/reject", async (
    Guid id,
    IApprovalService approvalService,
    CancellationToken cancellationToken) =>
{
    var result = await approvalService.RejectAsync(id, "development-user", cancellationToken);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.Run();

public partial class Program
{
}

public sealed record DelayShipmentRequest(string Reason);
public sealed record ShipmentInvestigationRequest(Guid ShipmentId);
