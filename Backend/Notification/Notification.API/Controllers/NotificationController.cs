using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notification.API.Contracts.Notifications;
using Notification.API.Filters;
using Notification.Application.Commands.Notifications.DeleteUserNotifications;
using Notification.Application.Commands.Notifications.ReadAllNotifications;
using Notification.Application.DTOs.Profiles;

namespace Notification.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator mediator;

        public NotificationController(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPatch("[action]")]
        [Authorize]
        [ServiceFilter(typeof(DecodeTokenFilter))]
        public async Task<IActionResult> ReadAllNotifications(CancellationToken cancellationToken)
        {
            var profile = (ProfileDTO)HttpContext.Items["profile"]!;

            await mediator.Send(new ReadAllNotificationsCommand
            {
                UserEmail = profile.Email
            }, cancellationToken);

            return NoContent();
        }

        [HttpDelete("[action]")]
        [Authorize]
        [ServiceFilter(typeof(DecodeTokenFilter))]
        public async Task<IActionResult> DeleteProfileNotifications(
            DeleteNotificationsRequest request,
            CancellationToken cancellationToken)
        {
            var profile = (ProfileDTO)HttpContext.Items["profile"]!;

            await mediator.Send(new DeleteUserNotificationsCommand
            {
                UserEmail = profile.Email,
                NotificationsId = request.NotificationsIdToDelete
            },
            cancellationToken);

            return NoContent();
        }

        [HttpPatch("[action]")]
        [Authorize]
        [ServiceFilter(typeof(DecodeTokenFilter))]
        public async Task<IActionResult> ReadNotification(
            ReadNotificationRequest request,
            CancellationToken cancellationToken)
        {
            return NoContent();
        }
    }
}
