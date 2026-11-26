using PicoChain.Domain.Entities;

namespace PicoChain.Tests;

public sealed class ApprovalRequestTests
{
    [Fact]
    public void Approve_records_resolution()
    {
        var request = new ApprovalRequest { ActionProposalId = Guid.NewGuid(), RequestedBy = "test" };

        request.Approve();

        Assert.Equal(ApprovalRequestStatus.Approved, request.Status);
        Assert.NotNull(request.ResolvedAt);
    }
}
