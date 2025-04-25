using Test.Application.Contracts.ProfileEntity;
using Test.Domain.Enums;

namespace Test.Application.Contracts.Test
{
    public class TestInfo
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedTime { get; set; }

        public bool IsPublic { get; set; }

        public string TestType { get; set; } = string.Empty;

        public double? DurationInMinutes { get; set; }

        public required ProfileResponse Owner { get; set; }
    }
}
