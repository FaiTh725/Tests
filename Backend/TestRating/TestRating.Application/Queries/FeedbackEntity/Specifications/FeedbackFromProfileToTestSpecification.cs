using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackEntity.Specifications
{
    public class FeedbackFromProfileToTestSpecification :
        Specification<Feedback>
    {
        public FeedbackFromProfileToTestSpecification(
            long profileId, long testId)
        {
            AddCriteria(feedback => 
                feedback.OwnerId == profileId &&
                feedback.TestId == testId);
        }
    }
}
