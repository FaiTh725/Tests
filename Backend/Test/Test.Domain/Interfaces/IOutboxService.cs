using Test.Domain.Primitives;

namespace Test.Domain.Interfaces
{
    public interface IOutboxService
    {
        Task AddOutboxMessage<T>(T message, IDatabaseSession? session = null, CancellationToken cancellationToken = default)
            where T : notnull;
    }
}
