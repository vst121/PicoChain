namespace PicoChain.Application.Shipments;

public sealed record ShipmentListItem(
    Guid Id,
    string ShipmentNumber,
    Guid OrderId,
    Guid CarrierId,
    string Status,
    int EventCount);

public sealed record ShipmentDetails(
    Guid Id,
    string ShipmentNumber,
    Guid OrderId,
    Guid CarrierId,
    string Status,
    IReadOnlyList<ShipmentEventItem> Events);

public sealed record ShipmentEventItem(
    Guid Id,
    string EventType,
    DateTimeOffset OccurredAt,
    string Description);

public interface IShipmentQueryService
{
    Task<IReadOnlyList<ShipmentListItem>> ListAsync(
        string? status,
        CancellationToken cancellationToken = default);

    Task<ShipmentDetails?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
