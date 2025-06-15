using MediatR;
using TestRating.Application.Contacts.Feedback;

namespace TestRating.Application.Queries.FeedbackEntity.GetFeedbackWithOwner
{
    public class GetFeedbackWithOwnerQuery : IRequest<FeedbackWithReviewsResponse>
    {
        public long Id { get; set; }
    }
}
