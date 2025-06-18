namespace Test.API.Contracts.Test
{
    public class SendTestAnswerRequest
    {
        public long QuestionId { get; set; }

        public List<long> QuestionAnswersId { get; set; } = new List<long>();
    }
}
