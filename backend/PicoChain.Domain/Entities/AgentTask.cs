namespace PicoChain.Domain.Entities;

public enum AgentTaskStatus { Created, Running, WaitingForApproval, Completed, Failed }

public sealed class AgentTask
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid? OperationalTaskId { get; init; }
    public required string AgentName { get; init; }
    public required string TaskType { get; init; }
    public string? CorrelationId { get; init; }
    public string? TraceId { get; init; }
    public AgentTaskStatus Status { get; private set; } = AgentTaskStatus.Created;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; private set; }
}
