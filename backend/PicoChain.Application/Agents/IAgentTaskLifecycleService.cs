namespace PicoChain.Application.Agents;

public interface IAgentTaskLifecycleService
{
    Task<bool> StartAsync(Guid agentTaskId, CancellationToken cancellationToken = default);
    Task<bool> CompleteAsync(Guid agentTaskId, CancellationToken cancellationToken = default);
    Task<bool> FailAsync(Guid agentTaskId, string reason, CancellationToken cancellationToken = default);
}
