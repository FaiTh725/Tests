using Test.Application.Contracts.ProfileAnswerEntity;

namespace Test.Application.Contracts.TestSession
{
    public class TempTestSession
    {
        public Guid Id { get; set; }

        public long TestId { get; set; }

        public long ProfileId { get; set; }

        public DateTime StartTime { get; set; }

        public double? TestDuration { get; set; }

        public List<SessionProfileAnswer> Answers { get; set; } = new List<SessionProfileAnswer>();

        // HangFire job id, if the user completes test before the time expires then
        // delete a job from hangfire
        public string? JobId { get; set; } = string.Empty;
    }
}
