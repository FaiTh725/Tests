using Notification.Application.DTOs.Notifications;

namespace Notification.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendNotification(NotificationDTO notification);

        Task NotifyNotificationsChanged(string notifcationsReceiver, List<NotificationDTO> newNotifications);
    }
}
