using Authorization.Domain.Repositories;

namespace Authorization.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public IRefreshTokenRepository RefreshTokenRepository { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        void BeginTransaction();

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        void CommitTransaction();

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        void RollBackTransaction();

        Task RollBackTransactionAsync(CancellationToken cancellationToken = default);

        bool CanConnect();

        Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
    }
}
