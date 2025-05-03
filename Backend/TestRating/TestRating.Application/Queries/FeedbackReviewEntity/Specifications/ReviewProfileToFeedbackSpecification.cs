using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackReviewEntity.Specifications
{
    public class ReviewProfileToFeedbackSpecification : Specification<FeedbackReview>
    {
        public ReviewProfileToFeedbackSpecification(
            long feedbackId, long profileId)
        {
            AddCriteria(review => 
                review.ReviewedFeedbackId == feedbackId && 
                review.OwnerId == profileId);
        }
    }
}
