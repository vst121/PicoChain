namespace PicoChain.Domain.Entities;

public enum ShipmentStatus { Planned, InTransit, Delayed, Delivered, Exception }

public sealed class Shipment
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string ShipmentNumber { get; init; }
    public Guid OrderId { get; init; }
    public Guid CarrierId { get; init; }
    public ShipmentStatus Status { get; private set; } = ShipmentStatus.Planned;
    public List<ShipmentEvent> Events { get; } = [];

    public void MarkDelayed() => Status = ShipmentStatus.Delayed;
    public void MarkInTransit() => Status = ShipmentStatus.InTransit;
    public void MarkDelivered() => Status = ShipmentStatus.Delivered;
}
