using Test.Domain.Primitives;

namespace Test.Domain.Events
{
    public class TestDeletedEvent : IDomainEvent
    {
        public long TestId { get; }

        public TestDeletedEvent(long testId)
        {
            TestId = testId;
        }
    }
}
