using CSharpFunctionalExtensions;
using TestRating.Domain.Validators;

namespace TestRating.Domain.Entities
{
    public class FeedbackReport : Entity
    {
        public string ReportMessage { get; private set; }

        public Feedback ReportedFeedback { get; private set; }

        public DateTime CreatedTime { get; private set; }

        public Profile Reviewer {  get; private set; }

        public FeedbackReport() {}

        private FeedbackReport(
            string reportMessage, 
            Feedback reportedFeedback, 
            Profile reviewer)
        {
            ReportMessage = reportMessage;
            ReportedFeedback = reportedFeedback;
            Reviewer = reviewer;
        }
      
        public static Result<FeedbackReport> Initialize(
            string reportMessage,
            Feedback reportedFeedback,
            Profile reviewer)
        {
            if(reportedFeedback is null)
            {
                return Result.Failure<FeedbackReport>("Feedback is required");
            }

            if (reviewer is null)
            {
                return Result.Failure<FeedbackReport>("Review owner is required");
            }

            if(string.IsNullOrEmpty(reportMessage) ||
                reportMessage.Length < ReportValidator.MIN_REPORT_MESSAGE_LENGTH ||
                reportMessage.Length > ReportValidator.MAX_REPORT_MESSAGE_LENGTH)
            {
                return Result.Failure<FeedbackReport>("Message is empty or outside of " +
                    $"{ReportValidator.MIN_REPORT_MESSAGE_LENGTH} - {ReportValidator.MAX_REPORT_MESSAGE_LENGTH}");
            }

            return Result.Success(new FeedbackReport(
                reportMessage,
                reportedFeedback,
                reviewer));
        }
    }
}
