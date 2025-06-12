namespace Authorization.Contracts.Events.Profile
{
    public interface IFeedbackProfileCreateFailed
    {
        public Guid CorrelationId { get; }

        public string Email { get; }

        public string Name { get; }

        public string Reason { get; }
    }
}
