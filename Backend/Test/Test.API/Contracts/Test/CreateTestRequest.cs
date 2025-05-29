using Test.Domain.Enums;

namespace Test.API.Contracts.Test
{
    public class CreateTestRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsPublic { get; set; }

        public TestType TestType { get; set; }

        public double? DurationInMinutes { get; set; }
    }
}
