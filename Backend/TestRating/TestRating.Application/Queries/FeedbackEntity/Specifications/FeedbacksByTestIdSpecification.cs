using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackEntity.Specifications
{
    public class FeedbacksByTestIdSpecification :
        Specification<Feedback>
    {
        public FeedbacksByTestIdSpecification(
            long testId)
        {
            AddCriteria(feedback => feedback.TestId == testId);
        }
    }
}
