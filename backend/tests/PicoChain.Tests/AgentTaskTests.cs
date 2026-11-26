using PicoChain.Domain.Entities;

namespace PicoChain.Tests;

public sealed class AgentTaskTests
{
    [Fact]
    public void Complete_sets_completed_status_and_timestamp()
    {
        var task = new AgentTask
        {
            AgentName = "ShipmentAgent",
            TaskType = "ShipmentDelayInvestigation"
        };

        task.Start();
        task.Complete();

        Assert.Equal(AgentTaskStatus.Completed, task.Status);
        Assert.NotNull(task.CompletedAt);
    }

    [Fact]
    public void Fail_sets_failed_status_and_timestamp()
    {
        var task = new AgentTask
        {
            AgentName = "ShipmentAgent",
            TaskType = "ShipmentDelayInvestigation"
        };

        task.Fail();

        Assert.Equal(AgentTaskStatus.Failed, task.Status);
        Assert.NotNull(task.CompletedAt);
    }
}
