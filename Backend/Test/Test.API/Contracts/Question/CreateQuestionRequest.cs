using Test.API.Contracts.QuestionAnswer;
using Test.Domain.Enums;

namespace Test.API.Contracts.Question
{
    public class CreateQuestionRequest
    {
        public IFormFileCollection? QuestionImages { get; set; }

        public string TestQuestion { get; set; } = string.Empty;

        public int QuestionWeight { get; set; }

        public QuestionType QuestionType { get; set; }

        public long TestId { get; set; }

        public List<CreateQuestionAnswerRequest> Answers { get; set; } = new List<CreateQuestionAnswerRequest>();
    }
}
