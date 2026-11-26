using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Approvals;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Approvals;

public sealed class ApprovalQueryService(PicoChainDbContext dbContext) : IApprovalQueryService
{
    public async Task<IReadOnlyList<PendingApprovalItem>> ListPendingAsync(CancellationToken cancellationToken = default) =>
        await Query()
            .Where(item => item.Status == "Pending")
            .ToListAsync(cancellationToken);

    public Task<PendingApprovalItem?> GetAsync(Guid approvalRequestId, CancellationToken cancellationToken = default) =>
        Query().SingleOrDefaultAsync(item => item.ApprovalRequestId == approvalRequestId, cancellationToken);

    private IQueryable<PendingApprovalItem> Query() =>
        from approval in dbContext.ApprovalRequests.AsNoTracking()
        join proposal in dbContext.ActionProposals.AsNoTracking()
            on approval.ActionProposalId equals proposal.Id
        select new PendingApprovalItem(
            approval.Id,
            proposal.Id,
            proposal.AgentTaskId,
            proposal.ActionType,
            proposal.ParametersJson,
            proposal.EvidenceSummary,
            approval.Status.ToString(),
            approval.CreatedAt);
}
