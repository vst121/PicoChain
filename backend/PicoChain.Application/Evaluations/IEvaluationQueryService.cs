namespace PicoChain.Application.Evaluations;

public sealed record EvaluationListItem(Guid Id, Guid AgentTaskId, string ScenarioId, string EvaluatorType, double OverallScore, bool Passed, DateTimeOffset CreatedAt);

public interface IEvaluationQueryService
{
    Task<IReadOnlyList<EvaluationListItem>> ListAsync(CancellationToken cancellationToken = default);
}
