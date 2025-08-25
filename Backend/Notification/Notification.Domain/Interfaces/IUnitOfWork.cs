using Notification.Domain.Primitives;
using Notification.Domain.Repositories;

namespace Notification.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        INotificationRepository NotificationRepository { get; }

        IDatabaseTransaction BeginTransaction();

        Task<IDatabaseTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        void CommitTransaction(IDatabaseTransaction session);

        Task CommitTransactionAsync(IDatabaseTransaction session, CancellationToken cancellationToken = default);

        void RollBackTransaction(IDatabaseTransaction session);

        Task RollBackTransactionAsync(IDatabaseTransaction session, CancellationToken cancellationToken = default);
    }
}
