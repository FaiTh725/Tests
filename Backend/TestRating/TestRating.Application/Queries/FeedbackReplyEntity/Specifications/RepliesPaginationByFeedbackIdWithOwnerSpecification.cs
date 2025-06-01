using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackReplyEntity.Specifications
{
    public class RepliesPaginationByFeedbackIdWithOwnerSpecification :
        Specification<FeedbackReply>
    {
        public RepliesPaginationByFeedbackIdWithOwnerSpecification(
            long feedbackId, int page, int pageSize)
        {
            AddCriteria(reply => reply.FeedbackId == feedbackId);
            AddInclude(reply => reply.Owner);

            EnablePagination(page, pageSize);
        }
    }
}
