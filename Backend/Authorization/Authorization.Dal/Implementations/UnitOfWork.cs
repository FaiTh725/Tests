using Authorization.Dal.Repositories;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Authorization.Dal.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;

        private readonly Lazy<IUserRepository> userRepository;
        private readonly Lazy<IRoleRepository> roleRepository;
        private readonly Lazy<IRefreshTokenRepository> refreshTokenRepository;

        private IDbContextTransaction transaction;
        private bool disposed = false;

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

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            transaction = await context.Database
                .BeginTransactionAsync(cancellationToken);
        }

        public bool CanConnect()
        {
            return context.Database.CanConnect();
        }

        public async Task<bool> CanConnectAsync(
            CancellationToken cancellationToken = default)
        {
            return await context.Database
                .CanConnectAsync(cancellationToken);
        }

        public void CommitTransaction()
        {
            AssuranceTransaction();

            transaction.Commit();
            transaction.Dispose();
        }

        public async Task CommitTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            AssuranceTransaction();

            await transaction.CommitAsync(cancellationToken);
            await transaction.DisposeAsync();
        }

        public void RollBackTransaction()
        {
            AssuranceTransaction();

            transaction.Rollback();
            transaction.Dispose();
        }

        public async Task RollBackTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            AssuranceTransaction();

            await transaction.RollbackAsync(cancellationToken);
            await transaction.DisposeAsync();
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void AssuranceTransaction()
        {
            if(transaction is null)
            {
                throw new InvalidOperationException("Transaction hasnt been started");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed && disposing)
            {
                context.Dispose();
                transaction?.Dispose();
            }
            disposed = true;
        }
    }
}
