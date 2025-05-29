using CSharpFunctionalExtensions;
using Test.Domain.Validators;

namespace Test.Domain.Entities
{
    public class TestSession: Entity
    {
        public long TestId { get; private set; }

        public long ProfileId { get; private set; }

        public DateTime StartTime { get; private set; }

        public DateTime? EndTime { get; private set; }

        public bool IsEnded { get; private set; }

        public int Percent {  get; private set; }

        private TestSession(
            long testId,
            long profileId)
        {
            TestId = testId;
            ProfileId = profileId;

            StartTime = DateTime.UtcNow;
            IsEnded = false;
        }

        public Result CloseSession(
            int percent)
        {
            if(IsEnded)
            {
                return Result.Failure("Session has already ended");
            }

            if(percent < TestSessionValidator.MIN_PERCENT ||
                percent > TestSessionValidator.MAX_PERCENT)
            {
                return Result.Failure("Percent outside from range " +
                    $"[{TestSessionValidator.MIN_PERCENT}, {TestSessionValidator.MAX_PERCENT}]");
            }

            EndTime = DateTime.UtcNow;
            IsEnded = true;
            Percent = percent;

            return Result.Success();
        }

        public static Result<TestSession> Initialize(
            long testId,
            long profileId)
        {
            return Result.Success(new TestSession(
                testId,
                profileId));
        }
    }
}
