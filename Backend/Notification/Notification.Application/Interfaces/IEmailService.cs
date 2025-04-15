using Notification.Application.DTO;

namespace Notification.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(EmailDTO email);
    }
}
