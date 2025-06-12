using TestRating.Application.Contacts.Profile;

namespace TestRating.Application.Contacts.FeedbackReport
{
    public class FeedbackReportWithReviewer
    {
        public long Id { get; set; }

        public string ReportMessage { get; set; } = string.Empty;

        public required BaseProfileResponse Reviewer { get; set; }

        public bool? IsApproval { get; set; }

        public DateTime CreatedTime { get; set; }

        public long FeedbackId { get; set; }
    }
}
