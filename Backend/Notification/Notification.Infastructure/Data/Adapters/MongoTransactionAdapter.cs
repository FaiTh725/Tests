using MongoDB.Driver;
using Notification.Domain.Primitives;

namespace Notification.Infrastructure.Data.Adapters
{
    public class MongoTransactionAdapter : IDatabaseTransaction
    {
        private bool isClosed;
        private bool disposed = false;

        public IClientSessionHandle Session { get; init; }

        public bool IsClosed => isClosed;

        public MongoTransactionAdapter(
            IClientSessionHandle session)
        {
            Session = session;

            isClosed = false;
        }
        ~MongoTransactionAdapter()
        {
            Dispose(false);
        }

        public void CloseSession()
        {
            isClosed = true;
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
                    if (!isClosed && Session.IsInTransaction)
                    {
                        Session.AbortTransaction();
                    }
                }
                catch
                {

                }
                finally
                {
                    Session.Dispose();
                }
            }

            disposed = true;
        }
    }
}
