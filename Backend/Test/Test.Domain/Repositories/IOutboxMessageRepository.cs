using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Domain.Repositories
{
    public interface IOutboxMessageRepository
    {
        Task<IEnumerable<OutboxMessage>> GetPendingMessages(CancellationToken cancellation = default);

        Task UpdateMessage(long id, OutboxMessage updatedMessage, IDatabaseSession? session = null, CancellationToken cancellationToken = default);
    }
}
