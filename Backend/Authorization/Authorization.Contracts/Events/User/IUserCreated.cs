namespace Authorization.Contracts.Events.User
{
    public interface IUserCreated
    {
        public Guid CorrelationId { get; }

        public string Email { get; }

        public string Name { get; }
    }
}
