using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Agents;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Agents;

public sealed class AgentTaskQueryService(PicoChainDbContext dbContext) : IAgentTaskQueryService
{
    public async Task<IReadOnlyList<AgentTaskListItem>> ListAsync(CancellationToken cancellationToken = default) =>
        await dbContext.AgentTasks
            .AsNoTracking()
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => new AgentTaskListItem(
                item.Id,
                item.AgentName,
                item.TaskType,
                item.Status.ToString(),
                item.CorrelationId,
                item.TraceId,
                item.CreatedAt))
            .ToListAsync(cancellationToken);

    public async Task<AgentTaskListItem?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.AgentTasks
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new AgentTaskListItem(
                item.Id,
                item.AgentName,
                item.TaskType,
                item.Status.ToString(),
                item.CorrelationId,
                item.TraceId,
                item.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
}
