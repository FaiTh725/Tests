using Test.Application.Contracts.QuestionAnswerEntity;

namespace Test.Application.Contracts.Question
{
    public class QuestionToPassTest
    {
        public long Id { get; set; }

        public string TestQuestion { get; set; } = string.Empty;

        public string QuestionType { get; set; } = string.Empty;

        public List<string> QuestionImages { get; set; } = new List<string>();

        public List<QuestionAnswerToPassTest> Answers { get; set; } = new List<QuestionAnswerToPassTest>();
    }
}
