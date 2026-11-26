namespace PicoChain.Domain.Entities;

public sealed class ShipmentEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ShipmentId { get; init; }
    public required string EventType { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public string Description { get; init; } = string.Empty;
}
