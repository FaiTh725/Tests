using Notification.Domain.Entities;
using Notification.Domain.Primitives;

namespace Notification.Application.Queries.Notifications.Specifications
{
    public class GetNotificationsByUserEmailSpecification : 
        BaseSpecification<NotificationEntity>
    {
        public GetNotificationsByUserEmailSpecification(
            string userEmail, int page, int pageSize)
        {
            Criteria = notification => notification.UserEmail == userEmail;

            OrderByDescendingExpression = notification => notification.SendTime;

            IsEnablePagination = true;
            Page = page;
            PageSize = pageSize;
        }
    }
}
