using MediatR;
using TestRating.Application.Contacts.FeedbackReply;
using TestRating.Application.Contacts.Pagination;

namespace TestRating.Application.Queries.FeedbackReplyEntity.GetFeedbackReplies
{
    public class GetFeedbackRepliesQuery: 
        IRequest<BasePaginationResponse<FeedbackReplyWithOwner>>
    {
        public long FeedbackId { get; set; }

        public int Page {  get; set; }

        public int PageSize { get; set; }
    }
}
