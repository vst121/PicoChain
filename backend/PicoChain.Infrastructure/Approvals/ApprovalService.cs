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
            await ExecuteApprovedActionAsync(proposal, cancellationToken);
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

    private async Task ExecuteApprovedActionAsync(ActionProposal proposal, CancellationToken cancellationToken)
    {
        if (proposal.ActionType != "CarrierContact")
            throw new InvalidOperationException($"Unsupported approved action type: {proposal.ActionType}");

        var parameters = System.Text.Json.JsonSerializer.Deserialize<CarrierContactParameters>(proposal.ParametersJson)
            ?? throw new InvalidOperationException("Invalid carrier contact parameters.");
        var shipment = await dbContext.Shipments.SingleOrDefaultAsync(x => x.Id == parameters.ShipmentId, cancellationToken)
            ?? throw new InvalidOperationException("Shipment for approved action was not found.");
        shipment.Events.Add(new ShipmentEvent
        {
            ShipmentId = shipment.Id,
            EventType = "CarrierContactRequested",
            OccurredAt = DateTimeOffset.UtcNow,
            Description = parameters.RecommendedAction
        });
        proposal.Execute();
        dbContext.AuditRecords.Add(new AuditRecord
        {
            Action = "ApprovedActionExecuted",
            EntityType = nameof(ActionProposal),
            EntityId = proposal.Id,
            Actor = "system",
            DetailsJson = $"{{\"shipmentId\":\"{shipment.Id}\"}}"
        });
    }

    private sealed record CarrierContactParameters(Guid ShipmentId, string RecommendedAction);
}
