using PicoChain.Infrastructure;
using PicoChain.Application.Shipments;
using PicoChain.Application.Approvals;

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

app.MapPost("/api/v1/approvals/{id:guid}/approve", async (
    Guid id,
    IApprovalService approvalService,
    CancellationToken cancellationToken) =>
{
    var result = await approvalService.ApproveAsync(id, "development-user", cancellationToken);
    return result is null ? Results.NotFound() : Results.Ok(result);
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
