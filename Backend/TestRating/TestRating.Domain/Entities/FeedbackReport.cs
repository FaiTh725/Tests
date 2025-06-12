using CSharpFunctionalExtensions;
using TestRating.Domain.Events;
using TestRating.Domain.Primitives;
using TestRating.Domain.Validators;

namespace TestRating.Domain.Entities
{
    public class FeedbackReport : DomainEventEntity
    {
        public string ReportMessage { get; private set; }

        public bool? IsApproval {  get; private set; }

        public Feedback ReportedFeedback { get; private set; }
        public long ReportedFeedbackId { get; private set; }

        public DateTime CreatedTime { get; private set; }

        public Profile Reviewer {  get; private set; }
        public long ReviewerId { get; private set; }

        public FeedbackReport() {}

        private FeedbackReport(
            string reportMessage,
            long reportedFeedbackId,
            long reviewerId)
        {
            ReportMessage = reportMessage;
            ReportedFeedbackId = reportedFeedbackId;
            ReviewerId = reviewerId;

            CreatedTime = DateTime.UtcNow;
            IsApproval = null;
        }

        public void ReviewReport(
            bool isApproved)
        {
            IsApproval = isApproved;

            if(isApproved == true)
            {
                RaiseDomainEvent(new FeedbackReportReviewedEvent
                { 
                    FeedbackReportId = Id
                });
            }
        }

        public static Result<FeedbackReport> Initialize(
            string reportMessage,
            long reportedFeedbackId,
            long reviewerId)
        {
            var isValid = Validate(reportMessage);

            if(isValid.IsFailure)
            {
                return Result.Failure<FeedbackReport>(isValid.Error);
            }

            return Result.Success(new FeedbackReport(
                reportMessage,
                reportedFeedbackId,
                reviewerId));
        }

        private static Result Validate(
            string reportMessage)
        {
            if (string.IsNullOrEmpty(reportMessage) ||
                reportMessage.Length < ReportValidator.MIN_REPORT_MESSAGE_LENGTH ||
                reportMessage.Length > ReportValidator.MAX_REPORT_MESSAGE_LENGTH)
            {
                return Result.Failure("Message is empty or outside of " +
                    $"{ReportValidator.MIN_REPORT_MESSAGE_LENGTH} - {ReportValidator.MAX_REPORT_MESSAGE_LENGTH}");
            }

            return Result.Success();
        }
    }
}
