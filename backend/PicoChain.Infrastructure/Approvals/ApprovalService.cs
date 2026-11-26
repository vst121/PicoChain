using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Approvals;
using PicoChain.Domain.Entities;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Approvals;

public sealed class ApprovalService(PicoChainDbContext dbContext) : IApprovalService
{
    public Task<ApprovalDecisionResult?> ApproveAsync(
        Guid approvalRequestId,
        string actor,
        CancellationToken cancellationToken = default) =>
        ResolveAsync(approvalRequestId, actor, true, cancellationToken);

    public Task<ApprovalDecisionResult?> RejectAsync(
        Guid approvalRequestId,
        string actor,
        CancellationToken cancellationToken = default) =>
        ResolveAsync(approvalRequestId, actor, false, cancellationToken);

    private async Task<ApprovalDecisionResult?> ResolveAsync(
        Guid approvalRequestId,
        string actor,
        bool approve,
        CancellationToken cancellationToken)
    {
        var request = await dbContext.ApprovalRequests
            .SingleOrDefaultAsync(item => item.Id == approvalRequestId, cancellationToken);

        if (request is null || request.Status != ApprovalRequestStatus.Pending)
        {
            return null;
        }

        var proposal = await dbContext.ActionProposals
            .SingleOrDefaultAsync(item => item.Id == request.ActionProposalId, cancellationToken);

        if (proposal is null || proposal.Status != ActionProposalStatus.Proposed)
        {
            return null;
        }

        if (approve)
        {
            request.Approve();
            proposal.Approve();
        }
        else
        {
            request.Reject();
            proposal.Reject();
        }

        dbContext.AuditRecords.Add(new AuditRecord
        {
            Action = approve ? "ApprovalGranted" : "ApprovalRejected",
            EntityType = nameof(ApprovalRequest),
            EntityId = request.Id,
            Actor = actor,
            DetailsJson = $"{{\"proposalId\":\"{proposal.Id}\"}}"
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ApprovalDecisionResult(request.Id, request.Status.ToString(), proposal.Status.ToString());
    }
}
