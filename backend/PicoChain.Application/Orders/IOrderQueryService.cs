namespace PicoChain.Application.Orders;

public sealed record OrderListItem(Guid Id, string OrderNumber, Guid CustomerId, string Status, int ItemCount);

public interface IOrderQueryService
{
    Task<IReadOnlyList<OrderListItem>> ListAsync(CancellationToken cancellationToken = default);
}
