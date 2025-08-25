using Test.Application.Contracts.Question;

namespace Test.Application.Contracts.TestSession
{
    public class SessionInfo
    {
        public long Id { get; set; }

        public long TestId { get; set; }

        public string TestName { get; set; } = string.Empty;

        public long ProfileId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsEnded { get; set; }

        public int Percent { get; set; }

        public List<QuestionProfileAnswers> QuestionsDetailResult { get; set; } = new List<QuestionProfileAnswers>();

    }
}
