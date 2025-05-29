using Notification.Domain.Repositories;

namespace Notification.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        INotificationRepository NotificationRepository { get; }


    }
}
