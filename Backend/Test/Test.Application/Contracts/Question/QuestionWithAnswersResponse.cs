using Test.Application.Contracts.QuestionAnswerEntity;
using Test.Domain.Enums;

namespace Test.Application.Contracts.Question
{
    public class QuestionWithAnswersResponse
    {
        public long Id { get; set; }

        public string TestQuestion { get; set; } = string.Empty;

        public int QuestionWeight { get; set; }

        public QuestionType QuestionType { get; set; }

        public List<string> QuestionImages { get; set; }= new List<string>();

        public List<QuestionAnswerResponse> Answers { get; set; } = new List<QuestionAnswerResponse>();
    }
}
