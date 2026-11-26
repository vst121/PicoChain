namespace PicoChain.Application.Evaluations;

public sealed record EvaluationListItem(Guid Id, Guid AgentTaskId, string ScenarioId, string EvaluatorType, double OverallScore, bool Passed, DateTimeOffset CreatedAt, string AgentVersion, string PromptVersion, string ModelIdentifier, int InputTokens, int OutputTokens, double EstimatedCost);

public interface IEvaluationQueryService
{
    Task<IReadOnlyList<EvaluationListItem>> ListAsync(CancellationToken cancellationToken = default);
}
