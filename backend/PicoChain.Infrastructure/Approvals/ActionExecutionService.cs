using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Approvals;
using PicoChain.Domain.Entities;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Approvals;

public sealed class ActionExecutionService(PicoChainDbContext dbContext) : IActionExecutionService
{
    public async Task<ExecuteActionResult?> ExecuteAsync(Guid proposalId, string actor, CancellationToken cancellationToken = default)
    {
        var proposal = await dbContext.ActionProposals.SingleOrDefaultAsync(item => item.Id == proposalId, cancellationToken);
        if (proposal is null || proposal.Status != ActionProposalStatus.Approved) return null;
        if (!string.Equals(proposal.ActionType, "UpdateShipmentStatus", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Unsupported action type: {proposal.ActionType}.");

        var parameters = JsonSerializer.Deserialize<UpdateShipmentStatusParameters>(proposal.ParametersJson)
            ?? throw new InvalidOperationException("Action parameters are invalid.");
        var shipment = await dbContext.Shipments.SingleOrDefaultAsync(item => item.Id == parameters.ShipmentId, cancellationToken);
        if (shipment is null) return null;
        if (!Enum.TryParse<ShipmentStatus>(parameters.NewStatus, true, out var newStatus))
            throw new InvalidOperationException($"Unknown shipment status: {parameters.NewStatus}.");

        switch (newStatus)
        {
            case ShipmentStatus.InTransit: shipment.MarkInTransit(); break;
            case ShipmentStatus.Delivered: shipment.MarkDelivered(); break;
            case ShipmentStatus.Delayed: shipment.MarkDelayed(); break;
            default: throw new InvalidOperationException($"Unsupported target shipment status: {newStatus}.");
        }

        proposal.Execute();
        shipment.Events.Add(new ShipmentEvent
        {
            ShipmentId = shipment.Id,
            EventType = "ShipmentStatusUpdated",
            OccurredAt = DateTimeOffset.UtcNow,
            Description = $"Shipment status updated to {newStatus} after approved agent recommendation."
        });
        dbContext.AuditRecords.Add(new AuditRecord
        {
            Action = "ApprovedActionExecuted",
            EntityType = nameof(ActionProposal),
            EntityId = proposal.Id,
            Actor = actor,
            DetailsJson = $"{{\"shipmentId\":\"{shipment.Id}\",\"newStatus\":\"{newStatus}\"}}"
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ExecuteActionResult(proposal.Id, shipment.Id, shipment.Status.ToString(), proposal.Status.ToString());
    }

    private sealed record UpdateShipmentStatusParameters(Guid ShipmentId, string NewStatus);
}
