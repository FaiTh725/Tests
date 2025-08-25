using Notification.Domain.Entities;
using Notification.Domain.Primitives;

namespace Notification.Domain.Repositories
{
    public interface INotificationRepository
    {
        Task<NotificationEntity?> GetNotification(long notificationId, CancellationToken cancellationToken = default);

        Task<NotificationEntity> AddNotification(NotificationEntity notification, IDatabaseTransaction? transaction = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<NotificationEntity>> GetNotifications(CancellationToken cancellationToken = default);

        Task<IEnumerable<NotificationEntity>> GetNotificationsByCriteria(BaseSpecification<NotificationEntity> specification, CancellationToken cancellationToken = default);

        Task MarkUserNotificationsAsRead(string userEmail, IDatabaseTransaction? transaction = null, CancellationToken cancellationToken = default);
    
        Task DeleteNotifications(List<long> notificationsIdToDelete, IDatabaseTransaction? databaseTransaction = null, CancellationToken cancellationToken = default);

        Task UpdateNotification(long notificationIdToUpdate, NotificationEntity updatedNotification, IDatabaseTransaction? transaction = null, CancellationToken cancellationToken = default);
    }
}
