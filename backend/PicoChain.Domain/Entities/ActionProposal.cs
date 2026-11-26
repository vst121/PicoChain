namespace PicoChain.Domain.Entities;

public enum ActionProposalStatus { Proposed, Approved, Rejected, Executed, Failed }

public sealed class ActionProposal
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid AgentTaskId { get; init; }
    public required string ActionType { get; init; }
    public required string ParametersJson { get; init; }
    public string? EvidenceSummary { get; init; }
    public ActionProposalStatus Status { get; private set; } = ActionProposalStatus.Proposed;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public void Approve() => Status = ActionProposalStatus.Approved;
    public void Reject() => Status = ActionProposalStatus.Rejected;
}
