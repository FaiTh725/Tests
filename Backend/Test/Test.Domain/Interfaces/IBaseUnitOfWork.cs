using Test.Domain.Primitives;

namespace Test.Domain.Interfaces
{
    public interface IBaseUnitOfWork: IDisposable
    {
        IDatabaseSession BeginTransaction();

        Task<IDatabaseSession> BeginTransactionAsync(CancellationToken cancellationToken = default);

        void CommitTransaction(IDatabaseSession session);

        Task CommitTransactionAsync(IDatabaseSession session, CancellationToken cancellationToken = default);

        void RollBackTransaction(IDatabaseSession session);

        Task RollBackTransactionAsync(IDatabaseSession session, CancellationToken cancellationToken = default);
    }
}
