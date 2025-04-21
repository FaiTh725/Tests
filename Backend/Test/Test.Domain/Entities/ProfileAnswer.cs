using CSharpFunctionalExtensions;

namespace Test.Domain.Entities
{
    public class ProfileAnswer : Entity
    {
        public long SessionId { get; private set; }

        public long QuestionId { get; private set; }

        public long QuestionAnswerId { get; private set; }

        public DateTime SendTime { get; private set; }

        private ProfileAnswer(
            long sessionId,
            long questionId,
            long questionAnswerId)
        {
            SessionId = sessionId;
            QuestionId = questionId;
            QuestionAnswerId = questionAnswerId;

            SendTime = DateTime.UtcNow;
        }

        public static Result<ProfileAnswer> Initialize(
            long sessionId,
            long questionId,
            long questionAnswerId)
        {
            return Result.Success(new ProfileAnswer(
                sessionId,
                questionId,
                questionAnswerId));
        }
    }
}
