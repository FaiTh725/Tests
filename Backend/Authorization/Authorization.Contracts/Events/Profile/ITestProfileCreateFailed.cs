namespace Authorization.Contracts.Events.Profile
{
    public interface ITestProfileCreateFailed
    {
        public Guid CorrelationId { get; }

        public string Email { get; }

        public string Name { get; }

        public string Reason { get; }
    }
}
