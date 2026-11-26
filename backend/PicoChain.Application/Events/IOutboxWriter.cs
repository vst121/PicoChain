namespace PicoChain.Application.Events;

public interface IOutboxWriter
{
    Task AddAsync(
        string eventType,
        string payload,
        string? correlationId,
        string? traceParent,
        CancellationToken cancellationToken = default);
}
