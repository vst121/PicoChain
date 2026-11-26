namespace PicoChain.Application.Approvals;

public sealed record ApprovalDecisionResult(
    Guid ApprovalRequestId,
    string ApprovalStatus,
    string ProposalStatus);

public interface IApprovalService
{
    Task<ApprovalDecisionResult?> ApproveAsync(
        Guid approvalRequestId,
        string actor,
        CancellationToken cancellationToken = default);

    Task<ApprovalDecisionResult?> RejectAsync(
        Guid approvalRequestId,
        string actor,
        CancellationToken cancellationToken = default);
}
