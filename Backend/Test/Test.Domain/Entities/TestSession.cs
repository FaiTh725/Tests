using CSharpFunctionalExtensions;

namespace Test.Domain.Entities
{
    public class TestSession: Entity
    {
        public long TestId { get; private set; }

        public long ProfileId { get; private set; }

        public DateTime StartTime { get; private set; }

        public DateTime? EndTime { get; private set; }

        public bool IsEnded { get; private set; }

        private TestSession(
            long testId,
            long profileId)
        {
            TestId = testId;
            ProfileId = profileId;

            StartTime = DateTime.UtcNow;
            IsEnded = false;
        }

        public Result CloseSession()
        {
            if(IsEnded)
            {
                return Result.Failure("Session has already ended");
            }

            EndTime = DateTime.UtcNow;
            IsEnded = true;

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
