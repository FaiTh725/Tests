using Microsoft.EntityFrameworkCore;
using TestRating.Dal.Specification;
using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;
using TestRating.Domain.Repositories;

namespace TestRating.Dal.Repositories
{
    public class FeedbackReplyRepository : IFeedbackReplyRepository
    {
        private readonly AppDbContext context;

        public FeedbackReplyRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<FeedbackReply> AddReply(
            FeedbackReply reply, 
            CancellationToken cancellationToken = default)
        {
            var replyEntity = await context.Replies
                .AddAsync(reply, cancellationToken);

            return replyEntity.Entity;
        }

        public async Task<IEnumerable<FeedbackReply>> GetRepliesByCriteria(
            Specification<FeedbackReply> specification, 
            int page, 
            int pageSize, 
            CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator.GetQuery(
                context.Replies,
                specification)
                .Skip( (page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<FeedbackReply>> GetRepliesByCriteria(
            Specification<FeedbackReply> specification, 
            CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator.GetQuery(
                context.Replies,
                specification)
                .ToListAsync(cancellationToken);
        }

        public async Task<FeedbackReply?> GetReply(
            long id, 
            CancellationToken cancellationToken = default)
        {
            return await context.Replies
                .FirstOrDefaultAsync(x => x.Id == id,
                    cancellationToken);
        }

        public async Task<FeedbackReply?> GetReplyByCriteria(
            Specification<FeedbackReply> specification, 
            CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator.GetQuery(
                context.Replies,
                specification)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task HardDeleteReply(
            long replyId, 
            CancellationToken cancellationToken = default)
        {
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            await context.Replies
                .IgnoreQueryFilters()
                .Where(x => x.Id == replyId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task SoftDeleteReply(
            long replyId, 
            CancellationToken cancellationToken = default)
        {

            var reply = await GetReply(replyId, cancellationToken);

            if (reply is null)
            {
                return;
            }

            reply.Delete();
        }

        public async Task UpdateReply(
            long replyId, 
            FeedbackReply updatedReply, 
            CancellationToken cancellationToken = default)
        {
            await context.Replies
                .Where(x => x.Id == replyId)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(x => x.Text, updatedReply.Text),
                    cancellationToken);
        }
    }
}
