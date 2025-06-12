namespace TestRating.Contracts.Profile
{
    public class CreateFeedbackProfile
    {
        public Guid CorrelationId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
