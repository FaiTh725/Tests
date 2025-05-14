using CSharpFunctionalExtensions;
using Test.Domain.Enums;

namespace Test.Domain.Entities
{
    public class TestAccess: Entity
    {
        public long TestId { get; private set; }

        public long TargetEntityId { get; private set; }

        public TargetAccessEntityType TargetAccessEntityType { get; private set; }

        private TestAccess(
            long testId,
            long availableId,
            TargetAccessEntityType targetAccessEntityType)
        {
            TestId = testId;
            TargetEntityId = availableId;
            TargetAccessEntityType = targetAccessEntityType;
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
