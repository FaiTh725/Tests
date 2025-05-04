using TestRating.Application.Contacts.Profile;

namespace TestRating.Application.Contacts.FeedbackReply
{
    public class FeedbackReplyWithOwner
    {
        public long Id { get; set; }

        public string Text { get; set; } = string.Empty;
    
        public long FeedbackId { get; set; }

        public DateTime SendTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public required BaseProfileResponse Owner { get; set; }
    }
}
