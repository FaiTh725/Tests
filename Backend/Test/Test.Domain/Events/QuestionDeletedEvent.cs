using Test.Domain.Primitives;

namespace Test.Domain.Events
{
    public class QuestionDeletedEvent : IDomainEvent
    {
        public long QuestionId { get; }

        public QuestionDeletedEvent(
            long questionId)
        {
            QuestionId = questionId;
        }
    }
}
