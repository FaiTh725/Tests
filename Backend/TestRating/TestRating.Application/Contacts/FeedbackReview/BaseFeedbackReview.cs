namespace TestRating.Application.Contacts.FeedbackReview
{
    public class BaseFeedbackReview
    {
        public long Id { get; set; }

        public bool IsPositive { get; set; }

        public long OwnerId { get; set; }

        public long FeedbackId { get; set; }
    }
}
