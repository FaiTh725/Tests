namespace TestRating.API.Contracts.Feedback
{
    public class CreateFeedbackRequest
    {
        public IFormFileCollection? Images { get; set; }

        public string Text { get; set; } = string.Empty;

        public long TestId { get; set; }

        public int Rating { get; set; }
    }
}
