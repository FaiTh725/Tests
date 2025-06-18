using Test.Application.Contracts.ProfileAnswerEntity;
using Test.Application.Contracts.QuestionAnswerEntity;

namespace Test.Application.Contracts.Question
{
    public class QuestionProfileAnswers
    {
        public long QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public List<string> QuestionImages { get; set; } = new List<string>();

        public List<QuestionAnswerToPassTest> Answers { get; set; } = new List<QuestionAnswerToPassTest>();

        public List<ProfileQuestionAnswers> ProfileAnswers { get; set; } = new List<ProfileQuestionAnswers>();
    }
}
