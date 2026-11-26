namespace PicoChain.Application.Evaluations;

public sealed record HumanReviewInput(Guid EvaluationId, string Reviewer, string Decision, string Notes);
public sealed record HumanReviewResult(Guid ReviewId, Guid EvaluationId, string Reviewer, string Decision, string Notes);

public interface IHumanReviewService
{
    Task<HumanReviewResult> CreateAsync(HumanReviewInput input, CancellationToken cancellationToken = default);
}
