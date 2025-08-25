using CSharpFunctionalExtensions;

namespace Test.Domain.Entities
{
    public class ProfileAnswer : Entity
    {
        public long SessionId { get; private set; }

        public long QuestionId { get; private set; }

        public List<long> QuestionAnswersId { get; set; } = new List<long>();

        public DateTime SendTime { get; private set; }

        public bool IsCorrect {  get; private set; }

        private ProfileAnswer(
            long sessionId,
            long questionId,
            List<long> questionAnswersId,
            bool isCorrect)
        {
            SessionId = sessionId;
            QuestionId = questionId;
            QuestionAnswersId = questionAnswersId;
            IsCorrect = isCorrect;

            SendTime = DateTime.UtcNow;
        }

        public static Result<ProfileAnswer> Initialize(
            long sessionId,
            long questionId,
            List<long> questionAnswersId,
            bool isCorrect)
        {
            if(questionAnswersId is null)
            {
                return Result.Failure<ProfileAnswer>("AnswersId is null");
            }

            return Result.Success(new ProfileAnswer(
                sessionId,
                questionId,
                questionAnswersId,
                isCorrect));
        }
    }
}
