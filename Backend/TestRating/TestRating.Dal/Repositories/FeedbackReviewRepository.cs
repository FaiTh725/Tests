using Microsoft.EntityFrameworkCore;
using TestRating.Dal.Specification;
using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;
using TestRating.Domain.Repositories;

namespace TestRating.Dal.Repositories
{
    public class FeedbackReviewRepository : IFeedbackReviewRepository
    {
        private readonly AppDbContext context;

        public FeedbackReviewRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<FeedbackReview> AddReview(
            FeedbackReview review, 
            CancellationToken cancellationToken = default)
        {
            var reviewEntity = await context.Reviews
                .AddAsync(review, cancellationToken);

            return reviewEntity.Entity;
        }

        public async Task DeleteReview(
            long reviewId, 
            CancellationToken cancellationToken = default)
        {
            await context.Reviews
                .Where(x => x.Id == reviewId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<FeedbackReview?> GetFeedbackReviewByCriteria(
            Specification<FeedbackReview> specification, 
            CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator.GetQuery(
                context.Reviews,
                specification)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<FeedbackReview?> GetReview(
            long id, 
            CancellationToken cancellationToken = default)
        {
            return await context.Reviews
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task UpdateReview(
            long id, 
            FeedbackReview updateReview, 
            CancellationToken cancellationToken = default)
        {
            await context.Reviews
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(x => x.IsPositive, updateReview.IsPositive),
                    cancellationToken);
        }
    }
}
