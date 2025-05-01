namespace Test.Contracts.Profile
{
    public class CreateTestProfile
    {
        public Guid CorrelationId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
