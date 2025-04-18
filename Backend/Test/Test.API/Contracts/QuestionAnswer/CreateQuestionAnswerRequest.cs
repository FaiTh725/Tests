
namespace Test.API.Contracts.QuestionAnswer
{
    public class CreateQuestionAnswerRequest
    {
        public IFormFileCollection? AnswerImages { get; set; }

        public string Answer { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}
