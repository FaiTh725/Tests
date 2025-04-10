using Authorization.Domain.Repositories;

namespace Authorization.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public IRefreshTokenRepository RefreshTokenRepository { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync();

        void BeginTransaction();

        Task BeginTransactionAsync();

        void CommitTransaction();

        Task CommitTransactionAsync();

        void RollBackTransaction();

        Task RollBackTransactionAsync();

        bool CanConnect();

        Task<bool> CanConnectAsync();
    }
}
