using Test.Application.Contracts.File;

namespace Test.Application.Contracts.QuestionAnswerEntity
{
    public class CreateQuestionAnswer
    {
        public List<FileModel> AnswerImages { get; set; } = new List<FileModel>();
        
        public string Answer { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}
