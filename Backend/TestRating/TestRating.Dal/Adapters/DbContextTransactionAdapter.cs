using Microsoft.EntityFrameworkCore.Storage;
using TestRating.Domain.Primitives;

namespace TestRating.Dal.Adapters
{
    public class DbContextTransactionAdapter : IDatabaseTransaction
    {
        private bool disposed;

        public IDbContextTransaction Transaction { get; init; }

        public bool IsInTransaction => Transaction
            .GetDbTransaction().Connection is not null;

        public DbContextTransactionAdapter(
            IDbContextTransaction transaction)
        {
            Transaction = transaction;
        }

        ~DbContextTransactionAdapter()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                try
                {
                    Transaction.Rollback();
                }
                catch
                {

                }
                finally
                {
                    Transaction.Dispose();
                }
            }

            disposed = true;
        }
    }
}
