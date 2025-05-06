using Test.Domain.Entities;

namespace Test.Domain.Repositories
{
    public interface IOutboxMessageRepository
    {
        Task<IEnumerable<OutboxMessage>> GetPendingMessages(CancellationToken cancellation = default);

        Task UpdateMessage(long id, OutboxMessage updatedMessage, CancellationToken cancellationToken = default);
    }
}
