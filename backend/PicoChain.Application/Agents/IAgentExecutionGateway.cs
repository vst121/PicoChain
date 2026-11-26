namespace PicoChain.Application.Agents;

public sealed record ShipmentAgentContext(
    Guid ShipmentId,
    string ShipmentNumber,
    string ShipmentStatus,
    Guid OrderId,
    Guid CarrierId,
    IReadOnlyList<ShipmentContextEvent> Events);

public sealed record ShipmentContextEvent(string Type, DateTimeOffset OccurredAt, string Description);

public sealed record AgentRecommendation(
    Guid AgentTaskId,
    string ActionType,
    string ParametersJson,
    string EvidenceSummary);

public sealed record ActionProposalResult(Guid ProposalId, Guid ApprovalRequestId, string Status);

public interface IAgentExecutionGateway
{
    Task<ShipmentAgentContext?> GetShipmentContextAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<ActionProposalResult?> SubmitRecommendationAsync(AgentRecommendation recommendation, CancellationToken cancellationToken = default);
}
