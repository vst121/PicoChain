using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Agents;
using PicoChain.Domain.Entities;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Agents;

public sealed class AgentExecutionGateway(PicoChainDbContext dbContext) : IAgentExecutionGateway
{
    public async Task<ShipmentAgentContext?> GetShipmentContextAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        var shipment = await dbContext.Shipments.AsNoTracking()
            .Include(item => item.Events)
            .SingleOrDefaultAsync(item => item.Id == shipmentId, cancellationToken);

        return shipment is null
            ? null
            : new ShipmentAgentContext(
                shipment.Id,
                shipment.ShipmentNumber,
                shipment.Status.ToString(),
                shipment.OrderId,
                shipment.CarrierId,
                shipment.Events.Select(item => new ShipmentContextEvent(item.EventType, item.OccurredAt, item.Description)).ToList());
    }

    public async Task<ActionProposalResult?> SubmitRecommendationAsync(
        AgentRecommendation recommendation,
        CancellationToken cancellationToken = default)
    {
        var agentTaskExists = await dbContext.AgentTasks.AnyAsync(item => item.Id == recommendation.AgentTaskId, cancellationToken);
        if (!agentTaskExists)
        {
            return null;
        }

        var proposal = new ActionProposal
        {
            AgentTaskId = recommendation.AgentTaskId,
            ActionType = recommendation.ActionType,
            ParametersJson = recommendation.ParametersJson,
            EvidenceSummary = recommendation.EvidenceSummary
        };
        var approval = new ApprovalRequest
        {
            ActionProposalId = proposal.Id,
            RequestedBy = "agent-service"
        };

        dbContext.ActionProposals.Add(proposal);
        dbContext.ApprovalRequests.Add(approval);
        dbContext.AuditRecords.Add(new AuditRecord
        {
            Action = "ActionProposalCreated",
            EntityType = nameof(ActionProposal),
            EntityId = proposal.Id,
            Actor = "agent-service",
            DetailsJson = $"{{\"agentTaskId\":\"{recommendation.AgentTaskId}\"}}"
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ActionProposalResult(proposal.Id, approval.Id, proposal.Status.ToString());
    }
}
