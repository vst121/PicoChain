using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Agents;
using PicoChain.Domain.Entities;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Agents;

public sealed class AgentTaskService(PicoChainDbContext dbContext) : IAgentTaskService
{
    public async Task<AgentTaskCreatedResult> CreateShipmentInvestigationAsync(
        Guid shipmentId,
        string correlationId,
        string? traceId,
        CancellationToken cancellationToken = default)
    {
        var shipmentExists = await dbContext.Shipments
            .AnyAsync(item => item.Id == shipmentId, cancellationToken);

        if (!shipmentExists)
        {
            throw new InvalidOperationException($"Shipment {shipmentId} was not found.");
        }

        var operationalTask = new OperationalTask
        {
            TaskType = "ShipmentDelayInvestigation",
            Description = $"Investigate delayed shipment {shipmentId}"
        };
        var agentTask = new AgentTask
        {
            OperationalTaskId = operationalTask.Id,
            AgentName = "Orchestrator",
            TaskType = "ShipmentDelayInvestigation",
            CorrelationId = correlationId,
            TraceId = traceId
        };

        dbContext.OperationalTasks.Add(operationalTask);
        dbContext.AgentTasks.Add(agentTask);
        dbContext.AuditRecords.Add(new AuditRecord
        {
            Action = "AgentTaskCreated",
            EntityType = nameof(AgentTask),
            EntityId = agentTask.Id,
            CorrelationId = correlationId,
            Actor = "system",
            DetailsJson = $"{{\"shipmentId\":\"{shipmentId}\"}}"
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AgentTaskCreatedResult(
            operationalTask.Id,
            agentTask.Id,
            agentTask.AgentName,
            agentTask.Status.ToString());
    }
}
