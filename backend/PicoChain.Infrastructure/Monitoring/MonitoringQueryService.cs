using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Monitoring;
using PicoChain.Domain.Entities;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Monitoring;

public sealed class MonitoringQueryService(PicoChainDbContext dbContext) : IMonitoringQueryService
{
    public async Task<MonitoringSummary> GetSummaryAsync(CancellationToken cancellationToken = default) =>
        new(
            await dbContext.Shipments.CountAsync(cancellationToken),
            await dbContext.Shipments.CountAsync(item => item.Status == ShipmentStatus.Delayed, cancellationToken),
            await dbContext.AgentTasks.CountAsync(item => item.Status == AgentTaskStatus.Running, cancellationToken),
            await dbContext.ApprovalRequests.CountAsync(item => item.Status == ApprovalRequestStatus.Pending, cancellationToken),
            await dbContext.OutboxMessages.CountAsync(item => item.PublishedAt == null, cancellationToken),
            DateTimeOffset.UtcNow);
}
