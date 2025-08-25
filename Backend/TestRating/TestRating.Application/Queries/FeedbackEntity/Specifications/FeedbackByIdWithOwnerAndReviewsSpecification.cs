using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackEntity.Specifications
{
    public class FeedbackByIdWithOwnerAndReviewsSpecification : 
        Specification<Feedback>
    {
        public FeedbackByIdWithOwnerAndReviewsSpecification(
            long feedbackId)
        {
            AddCriteria(feedback => feedback.Id == feedbackId);
            AddInclude(feedback => feedback.Owner);
            AddInclude(feedback => feedback.Reviews);
        }
    }
}
