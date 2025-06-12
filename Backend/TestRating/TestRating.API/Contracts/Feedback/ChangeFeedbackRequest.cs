
namespace TestRating.API.Contracts.Feedback
{
    public class ChangeFeedbackRequest
    {
        public long FeedbackId { get; set; }

        public IFormFileCollection? NewImages { get; set; }

        public string Text { get; set; } = string.Empty;

        public int Rating { get; set; }
    }
}
