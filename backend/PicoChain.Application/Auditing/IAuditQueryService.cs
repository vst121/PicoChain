namespace PicoChain.Application.Auditing;

public sealed record AuditItem(
    Guid Id,
    string Action,
    string EntityType,
    Guid EntityId,
    string? Actor,
    string? CorrelationId,
    DateTimeOffset OccurredAt,
    string? DetailsJson);

public interface IAuditQueryService
{
    Task<IReadOnlyList<AuditItem>> ListAsync(
        string? correlationId,
        int limit = 100,
        CancellationToken cancellationToken = default);
}
