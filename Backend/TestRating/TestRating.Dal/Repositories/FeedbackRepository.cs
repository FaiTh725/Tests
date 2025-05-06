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

        public async Task SoftDeleteFeedback(
            long id, 
            CancellationToken cancellationToken = default)
        {
            var report = await GetFeedbackById(id, cancellationToken);

            if (report is null)
            {
                return;
            }

            report.Delete();
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

        public async Task HardDeleteFeedback(
            long id, 
            CancellationToken cancellationToken = default)
        {
            await context.Feedbacks
                .IgnoreQueryFilters()
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<Feedback?> GetFeedbackExcludeFiltersById(
            long id, 
            CancellationToken cancellationToken = default)
        {
            var feedbacks = context.Feedbacks
                .AsQueryable();

            feedbacks = feedbacks.IgnoreQueryFilters();

            return await feedbacks
                    .FirstOrDefaultAsync(x => 
                        x.Id == id, 
                    cancellationToken);
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByCriteria(
            Specification<Feedback> specification, 
            int page, 
            int pageSize, 
            CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator.GetQuery(
                context.Feedbacks,
                specification)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacks(
            CancellationToken cancellationToken = default)
        {
            return await context.Feedbacks
                .ToListAsync(cancellationToken);
        }

        public async Task HardDeleteByCriteria(
            Specification<Feedback> specification, 
            CancellationToken cancellationToken = default)
        {
            if(specification.Criteria is not null)
            { 
                await context.Feedbacks
                    .IgnoreQueryFilters()
                    .Where(specification.Criteria)
                    .ExecuteDeleteAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksExcludeFiltersByCriteria(
            Specification<Feedback> specification, 
            CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator.GetQuery(
                context.Feedbacks.IgnoreQueryFilters(),
                specification)
                .ToListAsync(cancellationToken);
        }
    }
}
