using PicoChain.Application.Events;
using PicoChain.Domain.Events;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Events;

public sealed class OutboxWriter(PicoChainDbContext dbContext) : IOutboxWriter
{
    public async Task AddAsync(
        string eventType,
        string payload,
        string? correlationId,
        string? traceParent,
        CancellationToken cancellationToken = default)
    {
        dbContext.OutboxMessages.Add(new OutboxMessage
        {
            EventType = eventType,
            Payload = payload,
            CorrelationId = correlationId,
            TraceParent = traceParent
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
