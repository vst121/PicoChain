namespace PicoChain.Domain.Entities;

public sealed class Customer
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string CustomerNumber { get; init; }
    public required string Name { get; init; }
    public string Email { get; init; } = string.Empty;
}
