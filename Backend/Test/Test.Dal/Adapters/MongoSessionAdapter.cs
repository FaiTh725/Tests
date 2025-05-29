using MongoDB.Driver;
using Test.Domain.Primitives;

namespace Test.Dal.Adapters
{
    public class MongoSessionAdapter : IDatabaseSession
    {
        private bool isClosed;
        private bool disposed = false;
       
        public IClientSessionHandle Session { get; init; }

        public bool IsClosed => isClosed;

        public MongoSessionAdapter(
            IClientSessionHandle session)
        {
            Session = session;

            isClosed = false;
        }

        ~MongoSessionAdapter()
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
            if(disposed)
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
