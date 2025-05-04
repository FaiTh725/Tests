using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackReplyEntity.Specifications
{
    public class ReplyByOwnerAndFeedbackIdSpecification :
        Specification<FeedbackReply>
    {
        public ReplyByOwnerAndFeedbackIdSpecification(
            long ownerId, long feedbackId)
        {
            AddCriteria(reply => 
                reply.OwnerId == ownerId &&
                reply.FeedbackId == feedbackId);
        }
    }
}
