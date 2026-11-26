namespace PicoChain.Domain.Events;

public sealed class OutboxMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string EventType { get; init; }
    public required string Payload { get; init; }
    public string? CorrelationId { get; init; }
    public string? TraceParent { get; init; }
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? PublishedAt { get; private set; }
    public int Attempts { get; private set; }
    public string? LastError { get; private set; }

    public void MarkAttempt(string? error = null)
    {
        Attempts++;
        LastError = error;
    }

    public void MarkPublished() => PublishedAt = DateTimeOffset.UtcNow;
}
