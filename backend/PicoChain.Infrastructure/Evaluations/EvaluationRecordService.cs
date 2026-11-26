using PicoChain.Application.Evaluations;
using PicoChain.Domain.Entities;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Evaluations;

public sealed class EvaluationRecordService(PicoChainDbContext dbContext) : IEvaluationRecordService
{
    public async Task RecordAsync(EvaluationRecordInput input, CancellationToken cancellationToken = default)
    {
        dbContext.EvaluationRecords.Add(new EvaluationRecord
        {
            AgentTaskId = input.AgentTaskId,
            ScenarioId = input.ScenarioId,
            EvaluatorType = input.EvaluatorType,
            ScoresJson = input.ScoresJson,
            OverallScore = input.OverallScore,
            Passed = input.Passed,
            FailureReasonsJson = input.FailureReasonsJson
        });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
