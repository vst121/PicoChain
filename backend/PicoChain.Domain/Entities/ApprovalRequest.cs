namespace PicoChain.Domain.Entities;

public enum ApprovalRequestStatus { Pending, Approved, Rejected, Expired }

public sealed class ApprovalRequest
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ActionProposalId { get; init; }
    public required string RequestedBy { get; init; }
    public ApprovalRequestStatus Status { get; private set; } = ApprovalRequestStatus.Pending;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ResolvedAt { get; private set; }

    public void Approve()
    {
        Status = ApprovalRequestStatus.Approved;
        ResolvedAt = DateTimeOffset.UtcNow;
    }

    public void Reject()
    {
        Status = ApprovalRequestStatus.Rejected;
        ResolvedAt = DateTimeOffset.UtcNow;
    }
}
