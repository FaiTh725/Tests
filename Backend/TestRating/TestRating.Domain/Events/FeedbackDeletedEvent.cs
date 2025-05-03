using TestRating.Domain.Primitives;

namespace TestRating.Domain.Events
{
    public class FeedbackDeletedEvent : IDomainEvent
    {
        public long FeedbackId { get; set; }
    }
}
