using Notification.Domain.Interfaces;
using Notification.Domain.Primitives;
using Notification.Domain.Repositories;
using Notification.Infrastructure.Data.Adapters;
using Notification.Infrastructure.Data.Repositories;

namespace Notification.Infrastructure.Data.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;

        private readonly Lazy<INotificationRepository> notificationRepository;

        public UnitOfWork(
            AppDbContext context)
        {
            this.context = context;

            notificationRepository = new(() => new NotificationRepository(context));
        }

        public INotificationRepository NotificationRepository => notificationRepository.Value;

        public IDatabaseTransaction BeginTransaction()
        {
            var mongoSession = context.Client.StartSession();
            mongoSession.StartTransaction();

            return new MongoTransactionAdapter(mongoSession);
        }

        public async Task<IDatabaseTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            var mongoSession = await context.Client.StartSessionAsync(cancellationToken: cancellationToken);
            mongoSession.StartTransaction();

            return new MongoTransactionAdapter(mongoSession);
        }

        public void CommitTransaction(
            IDatabaseTransaction session)
        {
            var mongoSession = session as MongoTransactionAdapter;
            AssuranceTransaction(mongoSession);

            mongoSession!.Session.CommitTransaction();
            mongoSession.CloseSession();
        }

        public async Task CommitTransactionAsync(
            IDatabaseTransaction session, 
            CancellationToken cancellationToken = default)
        {
            var mongoSession = session as MongoTransactionAdapter;
            AssuranceTransaction(mongoSession);

            await mongoSession!.Session
                .CommitTransactionAsync(cancellationToken: cancellationToken);
            mongoSession.CloseSession();
        }

        public void Dispose()
        {
        }

        public void RollBackTransaction(
            IDatabaseTransaction session)
        {
            var mongoSession = session as MongoTransactionAdapter;
            AssuranceTransaction(mongoSession);

            mongoSession!.Session.AbortTransaction();
            mongoSession.CloseSession();
        }

        public async Task RollBackTransactionAsync(
            IDatabaseTransaction session, 
            CancellationToken cancellationToken = default)
        {
            var mongoSession = session as MongoTransactionAdapter;
            AssuranceTransaction(mongoSession);

            await mongoSession!.Session
                .AbortTransactionAsync(cancellationToken: cancellationToken);
            mongoSession.CloseSession();
        }

        private void AssuranceTransaction(MongoTransactionAdapter? mongoSession)
        {
            if (mongoSession?.Session.IsInTransaction != true)
            {
                throw new InvalidOperationException("Transaction is not started");
            }
        }
    }
}
