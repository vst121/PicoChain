using System.Threading.Channels;
using System.Collections.Concurrent;

public sealed class RealtimeEventHub
{
    private readonly ConcurrentDictionary<Guid, Channel<string>> _clients = new();

    public void Publish(string eventName, object payload)
    {
        var message = $"event: {eventName}\ndata: {System.Text.Json.JsonSerializer.Serialize(payload)}\n\n";
        foreach (var client in _clients.Values)
        {
            client.Writer.TryWrite(message);
        }
    }

    public async IAsyncEnumerable<string> ReadAllAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var clientId = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<string>();
        _clients[clientId] = channel;

        try
        {
            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return message;
            }
        }
        finally
        {
            _clients.TryRemove(clientId, out _);
            channel.Writer.TryComplete();
        }
    }
}
