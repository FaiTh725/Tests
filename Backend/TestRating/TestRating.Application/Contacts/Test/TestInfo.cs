using TestRating.Application.Contacts.Profile;

namespace TestRating.Application.Contacts.Test
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

        public required BaseProfileResponse Owner { get; set; }
    }
}
