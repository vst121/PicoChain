using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Shipments;
using PicoChain.Contracts.Events;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Shipments;

public sealed class ShipmentEventService(PicoChainDbContext dbContext) : IShipmentEventService
{
    public async Task<bool> MarkDelayedAsync(
        Guid shipmentId,
        string reason,
        string correlationId,
        string? traceParent,
        CancellationToken cancellationToken = default)
    {
        var shipment = await dbContext.Shipments
            .SingleOrDefaultAsync(item => item.Id == shipmentId, cancellationToken);

        if (shipment is null)
        {
            return false;
        }

        if (shipment.Status == Domain.Entities.ShipmentStatus.Delayed)
        {
            return true;
        }

        var occurredAt = DateTimeOffset.UtcNow;
        shipment.MarkDelayed();
        var shipmentEvent = new Domain.Entities.ShipmentEvent
        {
            ShipmentId = shipment.Id,
            EventType = "ShipmentDelayed",
            OccurredAt = occurredAt,
            Description = reason
        };
        dbContext.ShipmentEvents.Add(shipmentEvent);

        var delayedEvent = new ShipmentDelayedV1(
            Guid.NewGuid(),
            shipment.Id,
            shipment.ShipmentNumber,
            occurredAt,
            reason,
            correlationId,
            traceParent);

        dbContext.OutboxMessages.Add(new Domain.Events.OutboxMessage
        {
            EventType = "ShipmentDelayed.v1",
            Payload = JsonSerializer.Serialize(delayedEvent),
            CorrelationId = correlationId,
            TraceParent = traceParent,
            OccurredAt = occurredAt
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
