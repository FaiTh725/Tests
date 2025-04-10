using Authorization.Dal.Repositories;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Authorization.Dal.Implementations
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext context;

        private Lazy<IUserRepository> userRepository;
        private Lazy<IRoleRepository> roleRepository;
        private Lazy<IRefreshTokenRepository> refreshTokenRepository;

        private IDbContextTransaction transaction;

        public UnitOfWork(
            AppDbContext context)
        {
            this.context = context;

            userRepository = new Lazy<IUserRepository>(() => new UserRepository(this.context));
            roleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(this.context));
            refreshTokenRepository = new Lazy<IRefreshTokenRepository>(() => new RefreshTokenRepository(this.context));
        }

        public IUserRepository UserRepository => userRepository.Value;

        public IRoleRepository RoleRepository => roleRepository.Value;

        public IRefreshTokenRepository RefreshTokenRepository => refreshTokenRepository.Value;

        public void BeginTransaction()
        {
            transaction = context.Database.BeginTransaction();
        }

        public async Task BeginTransactionAsync()
        {
            transaction = await context.Database.BeginTransactionAsync();
        }

        public bool CanConnect()
        {
            return context.Database.CanConnect();
        }

        public async Task<bool> CanConnectAsync()
        {
            return await context.Database.CanConnectAsync();
        }

        public void CommitTransaction()
        {
            AssuranceTransaction();

            transaction.Commit();
            transaction.Dispose();
        }

        public async Task CommitTransactionAsync()
        {
            AssuranceTransaction();

            await transaction.CommitAsync();
            await transaction.DisposeAsync();
        }

        public void RollBackTransaction()
        {
            AssuranceTransaction();

            transaction.Rollback();
            transaction.Dispose();
        }

        public async Task RollBackTransactionAsync()
        {
            AssuranceTransaction();

            await transaction.RollbackAsync();
            await transaction.DisposeAsync();
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
            transaction?.Dispose();
        }

        private void AssuranceTransaction()
        {
            if(transaction is null)
            {
                throw new InvalidOperationException("Transaction hasnt been started");
            }
        }
    }
}
