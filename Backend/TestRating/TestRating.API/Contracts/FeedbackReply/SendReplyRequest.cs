namespace TestRating.API.Contracts.FeedbackReply
{
    public class SendReplyRequest
    {
        public long FeedbackId { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
