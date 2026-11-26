namespace PicoChain.Domain.Entities;

public enum OperationalTaskStatus { Open, InProgress, WaitingForApproval, Completed, Failed }

public sealed class OperationalTask
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string TaskType { get; init; }
    public string Description { get; init; } = string.Empty;
    public OperationalTaskStatus Status { get; private set; } = OperationalTaskStatus.Open;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
