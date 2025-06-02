using FluentValidation;
using Notification.API.Contracts.Notifications;

namespace Notification.API.Validators.Notifications
{
    public class DeleteNotificationsValidator : 
        AbstractValidator<DeleteNotificationsRequest>
    {
        public DeleteNotificationsValidator()
        {
            RuleFor(x => x.NotificationsIdToDelete)
                .NotEmpty()
                    .WithMessage("Notifications id list shouldnt be empty");
        }
    }
}
