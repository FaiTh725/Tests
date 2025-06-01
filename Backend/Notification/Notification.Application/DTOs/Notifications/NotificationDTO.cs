namespace Notification.Application.DTOs.Notifications
{
    public class NotificationDTO
    {
        public long Id { get; set; }

        public string ConsumerEmail { get; set; } = string.Empty;

        public string Title {  get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
