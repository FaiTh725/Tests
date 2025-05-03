namespace TestRating.API.Contracts.Feedback
{
    public class SendReviewOnFeedbackRequest
    {
        public long FeedbackId { get; set; }

        public bool IsPositive { get; set; }
    }
}
