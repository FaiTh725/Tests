using Notification.Application.DTOs.Emails;

namespace Notification.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(EmailDTO email);
    }
}
