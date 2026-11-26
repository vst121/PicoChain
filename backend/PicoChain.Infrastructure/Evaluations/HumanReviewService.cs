using Microsoft.EntityFrameworkCore;
using PicoChain.Application.Evaluations;
using PicoChain.Domain.Entities;
using PicoChain.Infrastructure.Persistence;

namespace PicoChain.Infrastructure.Evaluations;

public sealed class HumanReviewService(PicoChainDbContext dbContext) : IHumanReviewService
{
    public async Task<HumanReviewResult> CreateAsync(HumanReviewInput input, CancellationToken cancellationToken = default)
    {
        if (!await dbContext.EvaluationRecords.AnyAsync(item => item.Id == input.EvaluationId, cancellationToken))
            throw new InvalidOperationException("Evaluation was not found.");
        if (!Enum.TryParse<HumanReviewDecision>(input.Decision, true, out var decision))
            throw new InvalidOperationException("Review decision is invalid.");

        var review = new HumanReviewRecord
        {
            EvaluationId = input.EvaluationId,
            Reviewer = input.Reviewer,
            Decision = decision,
            Notes = input.Notes
        };
        dbContext.HumanReviewRecords.Add(review);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new HumanReviewResult(review.Id, review.EvaluationId, review.Reviewer, review.Decision.ToString(), review.Notes);
    }
}
