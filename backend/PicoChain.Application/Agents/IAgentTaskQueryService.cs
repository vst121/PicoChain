namespace PicoChain.Application.Agents;

public sealed record AgentTaskListItem(
    Guid Id,
    string AgentName,
    string TaskType,
    string Status,
    string? CorrelationId,
    string? TraceId,
    DateTimeOffset CreatedAt);

public interface IAgentTaskQueryService
{
    Task<IReadOnlyList<AgentTaskListItem>> ListAsync(CancellationToken cancellationToken = default);
    Task<AgentTaskListItem?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
