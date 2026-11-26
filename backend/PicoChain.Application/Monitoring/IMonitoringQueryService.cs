namespace PicoChain.Application.Monitoring;

public sealed record MonitoringSummary(
    int ShipmentCount,
    int DelayedShipmentCount,
    int RunningAgentTaskCount,
    int PendingApprovalCount,
    int PendingOutboxCount,
    DateTimeOffset GeneratedAt);

public interface IMonitoringQueryService
{
    Task<MonitoringSummary> GetSummaryAsync(CancellationToken cancellationToken = default);
}
