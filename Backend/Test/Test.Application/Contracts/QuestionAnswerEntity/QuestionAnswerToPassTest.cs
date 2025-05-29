namespace Test.Application.Contracts.QuestionAnswerEntity
{
    public class QuestionAnswerToPassTest
    {
        public long Id { get; set; }

        public List<string> QuestionAnswerImages { get; set; } = new List<string>();

        public string Answer { get; set; } = string.Empty;
    }
}
