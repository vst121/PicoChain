using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Orders;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Orders;

public sealed class OrderQueryService(PicoChainDbContext dbContext) : IOrderQueryService
{
    public async Task<IReadOnlyList<OrderListItem>> ListAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Orders.AsNoTracking().OrderBy(x => x.OrderNumber)
            .Select(x => new OrderListItem(x.Id, x.OrderNumber, x.CustomerId, x.Status.ToString(), x.Items.Count))
            .ToListAsync(cancellationToken);
}
