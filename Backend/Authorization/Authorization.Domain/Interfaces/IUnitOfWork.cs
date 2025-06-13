using Authorization.Domain.Primitives;
using Authorization.Domain.Repositories;
using System.Data;

namespace Authorization.Domain.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        public IUserRepository UserRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public IRefreshTokenRepository RefreshTokenRepository { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        IDatabaseTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        Task<IDatabaseTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

        void CommitTransaction(IDatabaseTransaction transaction);

        Task CommitTransactionAsync(IDatabaseTransaction transaction, CancellationToken cancellationToken = default);

        void RollBackTransaction(IDatabaseTransaction transaction);

        Task RollBackTransactionAsync(IDatabaseTransaction transaction, CancellationToken cancellationToken = default);
    }
}
