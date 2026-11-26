using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Auditing;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Auditing;

public sealed class AuditQueryService(PicoChainDbContext dbContext) : IAuditQueryService
{
    public async Task<IReadOnlyList<AuditItem>> ListAsync(
        string? correlationId,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 1, 500);
        var query = dbContext.AuditRecords.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            query = query.Where(item => item.CorrelationId == correlationId);
        }

        return await query
            .OrderByDescending(item => item.OccurredAt)
            .Take(limit)
            .Select(item => new AuditItem(
                item.Id,
                item.Action,
                item.EntityType,
                item.EntityId,
                item.Actor,
                item.CorrelationId,
                item.OccurredAt,
                item.DetailsJson))
            .ToListAsync(cancellationToken);
    }
}
