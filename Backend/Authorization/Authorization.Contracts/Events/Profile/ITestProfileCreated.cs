namespace Authorization.Contracts.Events.Profile
{
    public interface ITestProfileCreated
    {
        public Guid CorrelationId { get; }

        public long TestProfileId { get; }

        public string Email { get; }

        public string Name { get; }
    }
}
