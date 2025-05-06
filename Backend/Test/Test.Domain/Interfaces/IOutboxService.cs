namespace Test.Domain.Interfaces
{
    public interface IOutboxService
    {
        Task AddOutboxMessage<T>(T message, CancellationToken cancellationToken = default)
            where T : notnull;
    }
}
