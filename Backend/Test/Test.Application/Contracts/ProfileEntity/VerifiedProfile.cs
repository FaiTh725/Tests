namespace Test.Application.Contracts.ProfileEntity
{
    public class VerifiedProfile
    {
        public long Id { get; set; }

        public string Role { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
