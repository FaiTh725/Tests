namespace Notification.Contracts.Email
{
    public class SendEmailRequest
    {
        public string Consumer { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;
    }
}
