using Authorization.Dal.Adapters;
using Authorization.Dal.Repositories;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Primitives;
using Authorization.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Authorization.Dal.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;

        private readonly Lazy<IUserRepository> userRepository;
        private readonly Lazy<IRoleRepository> roleRepository;
        private readonly Lazy<IRefreshTokenRepository> refreshTokenRepository;

        // TODO: refactoring - not thread safety
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

        public IDatabaseTransaction BeginTransaction(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var transaction = context.Database
                .BeginTransaction(isolationLevel);

            return new DbContextTransactionAdapter(transaction);
        }

        public async Task<IDatabaseTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var transaction = await context.Database
                .BeginTransactionAsync(cancellationToken);

            return new DbContextTransactionAdapter(transaction);
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

        public void CommitTransaction(IDatabaseTransaction transaction)
        {
            var dbTransaction = transaction as DbContextTransactionAdapter;
            AssuranceTransaction(dbTransaction);

            dbTransaction!.Transaction.Commit();
        }

        public async Task CommitTransactionAsync(
            IDatabaseTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            var dbTransaction = transaction as DbContextTransactionAdapter;
            AssuranceTransaction(dbTransaction);

            await dbTransaction!.Transaction
                .CommitAsync(cancellationToken);
        }

        public void RollBackTransaction(
            IDatabaseTransaction transaction)
        {
            var dbTransaction = transaction as DbContextTransactionAdapter;
            AssuranceTransaction(dbTransaction);

            dbTransaction!.Transaction.Rollback();
        }

        public async Task RollBackTransactionAsync(
            IDatabaseTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            var dbTransaction = transaction as DbContextTransactionAdapter;
            AssuranceTransaction(dbTransaction);

            await dbTransaction!.Transaction
                .RollbackAsync(cancellationToken);
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
        }

        private void AssuranceTransaction(IDatabaseTransaction? transaction)
        {
            if (transaction is not null &&
                !transaction.IsInTransaction)
            {
                throw new InvalidOperationException("Transaction isnt started");
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
