namespace PicoChain.Domain.Entities;

public sealed class AuditRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Action { get; init; }
    public required string EntityType { get; init; }
    public Guid EntityId { get; init; }
    public string? Actor { get; init; }
    public string? CorrelationId { get; init; }
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public string? DetailsJson { get; init; }
}
