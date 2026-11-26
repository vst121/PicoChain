namespace PicoChain.Application.Approvals;

public sealed record PendingApprovalItem(
    Guid ApprovalRequestId,
    Guid ProposalId,
    Guid AgentTaskId,
    string ActionType,
    string ParametersJson,
    string? EvidenceSummary,
    string Status,
    DateTimeOffset CreatedAt);

public interface IApprovalQueryService
{
    Task<IReadOnlyList<PendingApprovalItem>> ListPendingAsync(CancellationToken cancellationToken = default);
    Task<PendingApprovalItem?> GetAsync(Guid approvalRequestId, CancellationToken cancellationToken = default);
}
