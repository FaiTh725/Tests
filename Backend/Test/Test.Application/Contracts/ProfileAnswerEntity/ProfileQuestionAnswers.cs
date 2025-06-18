namespace Test.Application.Contracts.ProfileAnswerEntity
{
    public class ProfileQuestionAnswers
    {
        public long QuestionId { get; set; }

        public List<long> ProfileAnswersId { get; set; } = new List<long>();

        public bool IsCorrectAnswer { get; set; }
    }
}
