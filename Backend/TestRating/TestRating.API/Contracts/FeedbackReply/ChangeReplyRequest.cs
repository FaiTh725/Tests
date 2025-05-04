namespace TestRating.API.Contracts.FeedbackReply
{
    public class ChangeReplyRequest
    {
        public long ReplyId { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
