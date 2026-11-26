namespace PicoChain.Application.Evaluations;

public sealed record EvaluationRecordInput(Guid AgentTaskId, string ScenarioId, string EvaluatorType, string ScoresJson, double OverallScore, bool Passed, string FailureReasonsJson, string AgentVersion, string PromptVersion, string ModelIdentifier, int InputTokens, int OutputTokens, double EstimatedCost);

public interface IEvaluationRecordService
{
    Task RecordAsync(EvaluationRecordInput input, CancellationToken cancellationToken = default);
}
