namespace PicoChain.Domain.Entities;

public sealed class Carrier
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string CarrierCode { get; init; }
    public required string Name { get; init; }
}
