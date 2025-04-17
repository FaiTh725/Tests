
namespace Test.Application.Contracts.QuestionAnswerEntity
{
    public class QuestionAnswerResponse
    {
        public long Id { get; set; }

        public List<string> QuestionAnswersImageUrls { get; set; } = new List<string>();

        public string Answer { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public long QuestionId { get; set; }
    }
}
