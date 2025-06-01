using Notification.Application.DTOs.Notifications;

namespace Notification.API.Hubs.Interfaces
{
    public interface INotification
    {
        Task Send(NotificationDTO notification);

        Task SendListNotifications(List<NotificationDTO> notifications);
    }
}
