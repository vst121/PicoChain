using Microsoft.EntityFrameworkCore;
using PicoChain.Domain.Entities;

namespace PicoChain.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task ApplyAsync(PicoChainDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Customers.AnyAsync(cancellationToken))
        {
            return;
        }

        var customer = new Customer
        {
            CustomerNumber = "CUS-1001",
            Name = "Northwind Retail",
            Email = "operations@northwind.example"
        };
        var carrier = new Carrier { CarrierCode = "DHL", Name = "DHL Freight" };
        var product = new Product { Sku = "SKU-1001", Name = "Industrial Sensor" };
        var order = new Order { OrderNumber = "ORD-1001", CustomerId = customer.Id };
        order.Items.Add(new OrderItem { OrderId = order.Id, ProductId = product.Id, Quantity = 4 });
        order.MarkValidated();

        var shipment = new Shipment
        {
            ShipmentNumber = "SHP-1001",
            OrderId = order.Id,
            CarrierId = carrier.Id
        };
        shipment.MarkDelayed();
        shipment.Events.Add(new ShipmentEvent
        {
            ShipmentId = shipment.Id,
            EventType = "ShipmentDelayed",
            OccurredAt = DateTimeOffset.UtcNow.AddHours(-6),
            Description = "Carrier reported an unexpected regional traffic disruption."
        });

        dbContext.AddRange(customer, carrier, product, order, shipment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
