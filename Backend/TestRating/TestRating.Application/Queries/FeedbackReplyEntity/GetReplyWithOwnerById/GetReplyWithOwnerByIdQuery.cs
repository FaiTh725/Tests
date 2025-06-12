using MediatR;
using TestRating.Application.Contacts.FeedbackReply;

namespace TestRating.Application.Queries.FeedbackReplyEntity.GetReplyWithOwnerById
{
    public class GetReplyWithOwnerByIdQuery :
        IRequest<FeedbackReplyWithOwner>
    {
        public long Id { get; set; }
    }
}
