using Notification.Domain.Entities;
using Notification.Domain.Primitives;

namespace Notification.Application.Queries.Notifications.Specifications
{
    public class GetNotificationsByIdListSpecifications : 
        BaseSpecification<NotificationEntity>
    {
        public GetNotificationsByIdListSpecifications(
            List<long> notificationsId)
        {
            Criteria = notification => notificationsId.Contains(notification.Id);
        }
    }
}
