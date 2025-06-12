using MediatR;
using TestRating.Application.Contacts.FeedbackReview;

namespace TestRating.Application.Queries.FeedbackReviewEntity.GetFeedbackReview
{
    public class GetFeedbackReviewQuery : 
        IRequest<BaseFeedbackReview>
    {
        public long Id { get; set; }
    }
}
