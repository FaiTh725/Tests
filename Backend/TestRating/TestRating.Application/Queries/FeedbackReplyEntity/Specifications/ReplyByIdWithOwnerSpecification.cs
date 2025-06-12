using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackReplyEntity.Specifications
{
    public class ReplyByIdWithOwnerSpecification :
        Specification<FeedbackReply>
    {
        public ReplyByIdWithOwnerSpecification(
            long replyId)
        {
            AddCriteria(reply => reply.Id == replyId);
            AddInclude(reply => reply.Owner);
        }
    }
}
