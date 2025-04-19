namespace Test.API.Contracts.Question
{
    public class UpdateQuestionRequest
    {
        public long Id { get; set; }

        public int QuestionWeight { get; set; }

        public string TestQuestion { get; set; } = string.Empty;
    }
}
