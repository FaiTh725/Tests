using MediatR;
using Notification.Application.DTOs.Notifications;

namespace Notification.Application.Queries.Notifications.GetUnreadNotifications
{
    public class GetUnreadNotificationsQuery : 
        IRequest<IEnumerable<NotificationDTO>>
    {
        public string UserEmail { get; set; } = string.Empty;

        public int Page {  get; set; }

        public int PageSize { get; set; }
    }
}
