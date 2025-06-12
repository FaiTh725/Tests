namespace Authorization.Contracts.Events.Profile
{
    public interface IFeedbackProfileCreated
    {
        public Guid CorrelationId { get; }

        public long FeedbackProfileId { get; }

        public string Email { get; }

        public string Name { get; }
    }
}
