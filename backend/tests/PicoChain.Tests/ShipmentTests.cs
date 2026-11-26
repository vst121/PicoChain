using PicoChain.Domain.Entities;

namespace PicoChain.Tests;

public sealed class ShipmentTests
{
    [Fact]
    public void MarkDelayed_sets_delayed_status()
    {
        var shipment = new Shipment { ShipmentNumber = "SHP-TEST", OrderId = Guid.NewGuid(), CarrierId = Guid.NewGuid() };

        shipment.MarkDelayed();

        Assert.Equal(ShipmentStatus.Delayed, shipment.Status);
    }
}
