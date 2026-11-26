using PicoChain.Domain.Events;

namespace PicoChain.Tests;

public sealed class OutboxMessageTests
{
    [Fact]
    public void MarkPublished_sets_publication_timestamp()
    {
        var message = new OutboxMessage
        {
            EventType = "ShipmentDelayed.v1",
            Payload = "{}"
        };

        message.MarkAttempt("temporary failure");
        message.MarkPublished();

        Assert.Equal(1, message.Attempts);
        Assert.NotNull(message.PublishedAt);
        Assert.Equal("temporary failure", message.LastError);
    }
}
