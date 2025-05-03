using MediatR;

namespace TestRating.Application.Commands.FeedbackReviewEntity.SendFeedbackReview
{
    public class SendFeedbackReviewCommand : 
        IRequest
    {
        public bool IsPositive { get; set; }

        public long ProfileId { get; set; }

        public long FeedbackId { get; set; }
    }
}
