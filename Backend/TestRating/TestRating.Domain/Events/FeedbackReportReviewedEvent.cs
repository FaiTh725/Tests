using TestRating.Domain.Primitives;

namespace TestRating.Domain.Events
{
    public class FeedbackReportReviewedEvent : IDomainEvent
    {
        public long FeedbackReportId {  get; set; }
    }
}
