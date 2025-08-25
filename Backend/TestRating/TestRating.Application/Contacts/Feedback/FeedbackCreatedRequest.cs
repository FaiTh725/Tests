namespace TestRating.Application.Contacts.Feedback
{
    public class FeedbackCreatedRequest
    {
        public long TestId { get; set; }

        public long FeedbackOwnerId { get; set; }
    }
}
