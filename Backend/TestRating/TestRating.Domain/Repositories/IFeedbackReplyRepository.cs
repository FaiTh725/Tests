using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Domain.Repositories
{
    public interface IFeedbackReplyRepository
    {
        Task<FeedbackReply> AddReply(FeedbackReply reply, CancellationToken cancellationToken = default);

        Task<FeedbackReply?> GetReply(long id, CancellationToken cancellationToken = default);

        Task<FeedbackReply?> GetReplyByCriteria(Specification<FeedbackReply> specification, CancellationToken cancellationToken = default);

        Task<IEnumerable<FeedbackReply>> GetRepliesByCriteria(Specification<FeedbackReply> specification, int page, int pageSize, CancellationToken cancellationToken = default);

        Task<IEnumerable<FeedbackReply>> GetRepliesByCriteria(Specification<FeedbackReply> specification, CancellationToken cancellationToken = default);

        Task UpdateReply(long replyId, FeedbackReply updatedReply, CancellationToken cancellationToken = default);

        Task SoftDeleteReply(long replyId, CancellationToken cancellationToken = default);

        Task HardDeleteReply(long replyId, CancellationToken cancellationToken = default);
    }
}
