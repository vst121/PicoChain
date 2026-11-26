namespace PicoChain.Contracts.Events;

public sealed record ShipmentDelayedV1(
    Guid EventId,
    Guid ShipmentId,
    string ShipmentNumber,
    DateTimeOffset OccurredAt,
    string Reason,
    string CorrelationId,
    string? TraceParent);
