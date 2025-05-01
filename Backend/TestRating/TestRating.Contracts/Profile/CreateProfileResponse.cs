namespace TestRating.Contracts.Profile
{
    public class CreateProfileResponse
    {
        public long Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
