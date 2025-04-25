namespace Test.Application.Contracts.ProfileAnswerEntity
{
    public class SessionProfileAnswer
    {
        public long QuestionId { get; set; }

        public List<long> QuestionAnswersId { get; set; } = new List<long>();

        public DateTime SendTime { get; set; }
    }
}
