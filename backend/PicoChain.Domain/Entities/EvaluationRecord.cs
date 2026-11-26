namespace PicoChain.Domain.Entities;

public sealed class EvaluationRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid AgentTaskId { get; init; }
    public required string ScenarioId { get; init; }
    public required string EvaluatorType { get; init; }
    public required string ScoresJson { get; init; }
    public double OverallScore { get; init; }
    public bool Passed { get; init; }
    public required string FailureReasonsJson { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
