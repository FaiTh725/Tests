using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackEntity.Specifications
{
    public class FeedbackByIdWithOwnerSpecification : 
        Specification<Feedback>
    {
        public FeedbackByIdWithOwnerSpecification(
            long feedbackId)
        {
            AddCriteria(feedback => feedback.Id == feedbackId);
            AddInclude(feedback => feedback.Owner);
        }
    }
}
