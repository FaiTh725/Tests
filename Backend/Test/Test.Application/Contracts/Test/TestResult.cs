using Test.Domain.Entities;

namespace Test.Application.Contracts.Test
{
    public class TestResult
    {
        public long TestId { get; set; }

        public int Percent {  get; set; }

        // Replace to another class
        public List<ProfileAnswer> ProfileAnswers { get; set; } = new List<ProfileAnswer>();
    }
}
