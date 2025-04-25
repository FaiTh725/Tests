using Test.Application.Contracts.Question;

namespace Test.Application.Contracts.Test
{
    public class TestToPassResponse
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string TestType { get; set; } = string.Empty;

        public List<QuestionToPassTest> Questions { get; set; } = new List<QuestionToPassTest>();
    }
}
