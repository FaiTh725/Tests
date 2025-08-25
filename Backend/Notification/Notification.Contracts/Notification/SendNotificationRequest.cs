namespace Notification.Contracts.Notification
{
    public class SendNotificationRequest
    {
        public string UserEmail { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
