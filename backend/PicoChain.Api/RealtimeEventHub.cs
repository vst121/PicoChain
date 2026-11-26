using System.Threading.Channels;

public sealed class RealtimeEventHub
{
    private readonly Channel<string> _events = Channel.CreateUnbounded<string>();

    public void Publish(string eventName, object payload)
        => _events.Writer.TryWrite($"event: {eventName}\ndata: {System.Text.Json.JsonSerializer.Serialize(payload)}\n\n");

    public IAsyncEnumerable<string> ReadAllAsync(CancellationToken cancellationToken)
        => _events.Reader.ReadAllAsync(cancellationToken);
}
