namespace PicoChain.Domain.Entities;

public enum HumanReviewDecision { Approved, Rejected, NeedsRevision }

public sealed class HumanReviewRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid EvaluationId { get; init; }
    public required string Reviewer { get; init; }
    public HumanReviewDecision Decision { get; init; }
    public string Notes { get; init; } = string.Empty;
    public DateTimeOffset ReviewedAt { get; init; } = DateTimeOffset.UtcNow;
}
