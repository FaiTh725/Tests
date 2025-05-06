namespace Test.Domain.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishPendingMessages();
    }
}
