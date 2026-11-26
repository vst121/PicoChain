using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Shipments;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Shipments;

public sealed class ShipmentQueryService(PicoChainDbContext dbContext) : IShipmentQueryService
{
    public async Task<IReadOnlyList<ShipmentListItem>> ListAsync(
        string? status,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Shipments.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<Domain.Entities.ShipmentStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(shipment => shipment.Status == parsedStatus);
        }

        return await query
            .OrderBy(shipment => shipment.ShipmentNumber)
            .Select(shipment => new ShipmentListItem(
                shipment.Id,
                shipment.ShipmentNumber,
                shipment.OrderId,
                shipment.CarrierId,
                shipment.Status.ToString(),
                shipment.Events.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<ShipmentDetails?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var shipment = await dbContext.Shipments
            .AsNoTracking()
            .Include(item => item.Events)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        return shipment is null
            ? null
            : new ShipmentDetails(
                shipment.Id,
                shipment.ShipmentNumber,
                shipment.OrderId,
                shipment.CarrierId,
                shipment.Status.ToString(),
                shipment.Events
                    .OrderByDescending(item => item.OccurredAt)
                    .Select(item => new ShipmentEventItem(
                        item.Id,
                        item.EventType,
                        item.OccurredAt,
                        item.Description))
                    .ToList());
    }
}
