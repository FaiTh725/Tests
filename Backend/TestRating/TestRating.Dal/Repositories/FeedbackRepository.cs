using Microsoft.EntityFrameworkCore;
using TestRating.Dal.Specification;
using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;
using TestRating.Domain.Repositories;

namespace TestRating.Dal.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly AppDbContext context;

        public FeedbackRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<Feedback> AddFeedback(
            Feedback feedback, 
            CancellationToken cancellationToken = default)
        {
            var addedFeedback = await context.Feedbacks
                .AddAsync(feedback, cancellationToken);

            return addedFeedback.Entity;
        }

        public async Task DeleteFeedback(
            long id, 
            CancellationToken cancellationToken)
        {
            await context.Feedbacks
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public Task<Feedback?> GetFeedbackByCriteria(
            Specification<Feedback> specification, 
            CancellationToken cancellationToken = default)
        {
            return SpecificationEvaluator.GetQuery(
                context.Feedbacks,
                specification)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Feedback?> GetFeedbackById(
            long id, 
            CancellationToken cancellationToken = default)
        {
            return await context.Feedbacks
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task UpdateFeedback(
            long feedbackId, 
            Feedback updatedFeedback, 
            CancellationToken cancellationToken = default)
        {
            await context.Feedbacks
                .Where(x => x.Id == feedbackId)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(x => x.Text, updatedFeedback.Text)
                    .SetProperty(x => x.Rating, updatedFeedback.Rating)
                    .SetProperty(x => x.UpdateTime, updatedFeedback.UpdateTime),
                cancellationToken);
        }
    }
}
