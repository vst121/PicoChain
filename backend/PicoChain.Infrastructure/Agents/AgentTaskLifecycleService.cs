using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Agents;
using PicoChain.Domain.Entities;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Agents;

public sealed class AgentTaskLifecycleService(PicoChainDbContext dbContext) : IAgentTaskLifecycleService
{
    public Task<bool> StartAsync(Guid agentTaskId, CancellationToken cancellationToken = default) =>
        TransitionAsync(agentTaskId, AgentTaskStatus.Running, null, cancellationToken);

    public Task<bool> CompleteAsync(Guid agentTaskId, CancellationToken cancellationToken = default) =>
        TransitionAsync(agentTaskId, AgentTaskStatus.Completed, null, cancellationToken);

    public Task<bool> FailAsync(Guid agentTaskId, string reason, CancellationToken cancellationToken = default) =>
        TransitionAsync(agentTaskId, AgentTaskStatus.Failed, reason, cancellationToken);

    private async Task<bool> TransitionAsync(
        Guid agentTaskId,
        AgentTaskStatus targetStatus,
        string? reason,
        CancellationToken cancellationToken)
    {
        var task = await dbContext.AgentTasks
            .SingleOrDefaultAsync(item => item.Id == agentTaskId, cancellationToken);

        if (task is null)
        {
            return false;
        }

        switch (targetStatus)
        {
            case AgentTaskStatus.Running:
                task.Start();
                break;
            case AgentTaskStatus.Completed:
                task.Complete();
                break;
            case AgentTaskStatus.Failed:
                task.Fail();
                break;
            default:
                throw new InvalidOperationException($"Unsupported lifecycle transition: {targetStatus}.");
        }

        dbContext.AuditRecords.Add(new AuditRecord
        {
            Action = $"AgentTask{targetStatus}",
            EntityType = nameof(AgentTask),
            EntityId = task.Id,
            CorrelationId = task.CorrelationId,
            Actor = "agent-service",
            DetailsJson = reason is null ? null : $"{{\"reason\":\"{reason.Replace("\"", "\\\"")}\"}}"
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
