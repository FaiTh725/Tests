namespace Notification.API.Contracts.Notifications
{
    public class DeleteNotificationsRequest
    {
        public List<long> NotificationsIdToDelete { get; set; } = new List<long>();
    }
}
