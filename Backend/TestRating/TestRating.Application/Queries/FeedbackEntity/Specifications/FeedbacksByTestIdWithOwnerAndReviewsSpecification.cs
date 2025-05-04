using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackEntity.Specifications
{
    public class FeedbacksByTestIdWithOwnerAndReviewsSpecification :
        Specification<Feedback>
    {
        public FeedbacksByTestIdWithOwnerAndReviewsSpecification(
            long testId)
        {
            AddCriteria(feedback => feedback.TestId == testId);
            AddInclude(feedback => feedback.Owner);
            AddInclude(feedback => feedback.Reviews);
            AddOrderByDescending(x => x.UpdateTime);
        }
    }
}
