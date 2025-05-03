using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Domain.Repositories
{
    public interface IFeedbackReviewRepository
    {
        Task<FeedbackReview> AddReview(FeedbackReview review, CancellationToken cancellationToken = default);

        Task<FeedbackReview?> GetReview(long id, CancellationToken cancellationToken = default);

        Task DeleteReview(long reviewId, CancellationToken cancellationToken = default);

        Task UpdateReview(long id, FeedbackReview updatedReview, CancellationToken cancellationToken = default);

        Task<FeedbackReview?> GetFeedbackReviewByCriteria(Specification<FeedbackReview> specification, CancellationToken cancellationToken = default);
    }
}
