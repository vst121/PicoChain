namespace PicoChain.Application.Shipments;

public interface IShipmentEventService
{
    Task<bool> MarkDelayedAsync(
        Guid shipmentId,
        string reason,
        string correlationId,
        string? traceParent,
        CancellationToken cancellationToken = default);
}
