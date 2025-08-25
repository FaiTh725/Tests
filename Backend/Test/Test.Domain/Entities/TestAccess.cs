using CSharpFunctionalExtensions;
using Test.Domain.Enums;
using Test.Domain.Events;
using Test.Domain.Primitives;

namespace Test.Domain.Entities
{
    public class TestAccess: DomainEventEntity
    {
        public long TestId { get; private set; }

        public long TargetEntityId { get; private set; }

        public TargetAccessEntityType TargetAccessEntityType { get; private set; }

        private TestAccess(
            long testId,
            long targetEntityId,
            TargetAccessEntityType targetAccessEntityType)
        {
            TestId = testId;
            TargetEntityId = targetEntityId;
            TargetAccessEntityType = targetAccessEntityType;
        }

        public void ProvideAccess()
        {
            RaiseDomainEvent(new NewTestAccessCreated
            {
                TestAccessId = Id
            });
        }

        public static Result<TestAccess> Initialize(
            long testId,
            long targetEntityId,
            TargetAccessEntityType targetAccessEntityType)
        {
            return Result.Success(new TestAccess(
                testId,
                targetEntityId,
                targetAccessEntityType));
        }
    }
}
