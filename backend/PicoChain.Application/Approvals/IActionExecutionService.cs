namespace PicoChain.Application.Approvals;

public sealed record ExecuteActionResult(Guid ProposalId, Guid ShipmentId, string ShipmentStatus, string ProposalStatus);

public interface IActionExecutionService
{
    Task<ExecuteActionResult?> ExecuteAsync(Guid proposalId, string actor, CancellationToken cancellationToken = default);
}
