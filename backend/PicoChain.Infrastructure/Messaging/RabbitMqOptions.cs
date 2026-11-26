namespace PicoChain.Infrastructure.Messaging;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string UserName { get; init; } = "picochain";
    public string Password { get; init; } = "development-only";
    public string Exchange { get; init; } = "picochain.events";
}
