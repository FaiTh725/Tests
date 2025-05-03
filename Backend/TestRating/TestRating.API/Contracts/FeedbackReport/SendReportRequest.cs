namespace TestRating.API.Contracts.FeedbackReport
{
    public class SendReportRequest
    {
        public string Message { get; set; } = string.Empty;

        public long ReportedFeedbackId { get; set; }
    }
}
