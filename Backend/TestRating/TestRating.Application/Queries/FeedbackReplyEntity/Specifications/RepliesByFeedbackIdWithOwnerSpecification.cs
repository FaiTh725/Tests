using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackReplyEntity.Specifications
{
    public class RepliesByFeedbackIdWithOwnerSpecification :
        Specification<FeedbackReply>
    {
        public RepliesByFeedbackIdWithOwnerSpecification(
            long feedbackId)
        {
            AddCriteria(reply => reply.FeedbackId == feedbackId);
            AddInclude(reply => reply.Owner);
        }
    }
}
