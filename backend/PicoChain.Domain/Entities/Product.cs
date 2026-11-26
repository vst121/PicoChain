namespace PicoChain.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Sku { get; init; }
    public required string Name { get; init; }
}
