namespace Test.Application.Contracts.ProfileEntity
{
    public class ConfirmedProfile
    {
        public long Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
