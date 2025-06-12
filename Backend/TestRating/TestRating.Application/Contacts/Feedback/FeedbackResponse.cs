using TestRating.Application.Contacts.Profile;

namespace TestRating.Application.Contacts.Feedback
{
    public class FeedbackResponse
    {
        public long Id { get; set; }

        public List<string> FeedbackImages { get; set; } = new List<string>();

        public string Text { get; set; } = string.Empty;

        public long TestId { get; set; }

        public required BaseProfileResponse Profile { get; set; }
        
        public int Rating { get; set; }

        public DateTime SendTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
