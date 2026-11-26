using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Evaluations;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Evaluations;

public sealed class EvaluationQueryService(PicoChainDbContext dbContext) : IEvaluationQueryService
{
    public async Task<IReadOnlyList<EvaluationListItem>> ListAsync(CancellationToken cancellationToken = default) =>
        await dbContext.EvaluationRecords.AsNoTracking().OrderByDescending(x => x.CreatedAt)
            .Select(x => new EvaluationListItem(x.Id, x.AgentTaskId, x.ScenarioId, x.EvaluatorType, x.OverallScore, x.Passed, x.CreatedAt, x.AgentVersion, x.PromptVersion, x.ModelIdentifier, x.InputTokens, x.OutputTokens, x.EstimatedCost))
            .ToListAsync(cancellationToken);
}
