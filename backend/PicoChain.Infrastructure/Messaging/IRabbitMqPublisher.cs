namespace PicoChain.Infrastructure.Messaging;

public interface IRabbitMqPublisher
{
    Task PublishAsync(
        string eventType,
        string payload,
        string? traceParent,
        CancellationToken cancellationToken = default);
}
