using Notification.Domain.Entities;
using Notification.Domain.Primitives;

namespace Notification.Application.Queries.Notifications.Specifications
{
    public class GetUnreadNotificationsByUserEmailSpecification : 
        BaseSpecification<NotificationEntity>
    {
        public GetUnreadNotificationsByUserEmailSpecification(
            string userEmail, int page, int pageSize)
        {
            Criteria = notification => 
                notification.UserEmail == userEmail && 
                notification.IsRead == false;

            OrderByDescendingExpression = notification => notification.SendTime;

            IsEnablePagination = true;
            Page = page;
            PageSize = pageSize;
        }
    }
}
