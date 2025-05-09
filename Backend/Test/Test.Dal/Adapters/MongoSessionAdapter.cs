using MongoDB.Driver;
using Test.Domain.Primitives;

namespace Test.Dal.Adapters
{
    public class MongoSessionAdapter : IDatabaseSession
    {
        private bool isClosed;

        public IClientSessionHandle Session { get; init; }

        public bool IsClosed => isClosed;

        public MongoSessionAdapter(
            IClientSessionHandle session)
        {
            Session = session;

            isClosed = false;
        }

        public void CloseSession()
        {
            isClosed = true;
        }

        public void Dispose()
        {
            if(!isClosed && Session.IsInTransaction)
            {
                try
                {
                    Session.AbortTransaction();
                }
                catch
                {

                }
            }

            Session.Dispose();
        }
    }
}
