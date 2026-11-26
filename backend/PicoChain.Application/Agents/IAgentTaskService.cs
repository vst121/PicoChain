namespace PicoChain.Application.Agents;

public sealed record AgentTaskCreatedResult(
    Guid OperationalTaskId,
    Guid AgentTaskId,
    string AgentName,
    string Status);

public interface IAgentTaskService
{
    Task<AgentTaskCreatedResult> CreateShipmentInvestigationAsync(
        Guid shipmentId,
        string correlationId,
        string? traceId,
        CancellationToken cancellationToken = default);
}
