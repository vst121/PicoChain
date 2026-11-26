namespace PicoChain.Domain.Entities;

public enum OrderStatus { Received, Validated, Incomplete, Cancelled, Fulfilled }

public sealed class Order
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string OrderNumber { get; init; }
    public Guid CustomerId { get; init; }
    public OrderStatus Status { get; private set; } = OrderStatus.Received;
    public List<OrderItem> Items { get; } = [];

    public void MarkValidated() => Status = OrderStatus.Validated;
    public void MarkIncomplete() => Status = OrderStatus.Incomplete;
}
