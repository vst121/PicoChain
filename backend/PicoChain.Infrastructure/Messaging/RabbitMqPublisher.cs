using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace PicoChain.Infrastructure.Messaging;

public sealed class RabbitMqPublisher(IOptions<RabbitMqOptions> options) : IRabbitMqPublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions settings = options.Value;
    private IConnection? connection;
    private IChannel? channel;

    public async Task PublishAsync(
        string eventType,
        string payload,
        string? traceParent,
        CancellationToken cancellationToken = default)
    {
        connection ??= await new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password
        }.CreateConnectionAsync(cancellationToken);

        channel ??= await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.ExchangeDeclareAsync(settings.Exchange, ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);

        var properties = new BasicProperties
        {
            Persistent = true,
            Type = eventType,
            Headers = traceParent is null ? null : new Dictionary<string, object?>
            {
                ["traceparent"] = Encoding.UTF8.GetBytes(traceParent)
            }
        };

        await channel.BasicPublishAsync(
            settings.Exchange,
            eventType,
            mandatory: false,
            basicProperties: properties,
            body: Encoding.UTF8.GetBytes(payload),
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (channel is not null)
        {
            await channel.DisposeAsync();
        }

        if (connection is not null)
        {
            await connection.DisposeAsync();
        }
    }
}
