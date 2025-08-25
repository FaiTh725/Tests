using Microsoft.AspNetCore.SignalR;
using Notification.API.Hubs.Instances;
using Notification.API.Hubs.Interfaces;
using Notification.Application.DTOs.Notifications;
using Notification.Application.Interfaces;

namespace Notification.API.Services
{
    public class HubNotificationSender : INotificationService
    {
        private readonly IHubContext<NotificationHub, INotification> hubContext;

        public HubNotificationSender(
            IHubContext<NotificationHub, INotification> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task NotifyNotificationsChanged(
            string notificationsReceiver,
            List<NotificationDTO> newNotifications)
        {
            await hubContext.Clients
                .User(notificationsReceiver)
                .SendListNotifications(newNotifications);
        }

        public async Task SendNotification(
            NotificationDTO notification)
        {
            await hubContext.Clients
                .User(notification.ConsumerEmail)
                .Send(notification);
        }
    }
}
