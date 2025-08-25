using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackEntity.Specifications
{
    public class FeedbacksByTestIdAndRatingWithOwnerSpecification : 
        Specification<Feedback>
    {
        public FeedbacksByTestIdAndRatingWithOwnerSpecification(
            long testId, int rating)
        {
            AddCriteria(x => 
                x.TestId == testId && 
                x.Rating == rating);

            AddInclude(x => x.Owner);
            AddInclude(x => x.Reviews);

            AddOrderByDescending(x => x.UpdateTime);
        }
    }
}
