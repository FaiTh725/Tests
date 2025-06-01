using CSharpFunctionalExtensions;
using Notification.Domain.Primitives;
using Notification.Domain.Validators;
using System.Text.RegularExpressions;

namespace Notification.Domain.Entities
{
    public class NotificationEntity : BaseEntity
    {
        // if use user id then id from other services may be not unique
        public string UserEmail { get; private set; }

        public string Title { get; private set; }

        public string Message { get; private set; }

        public DateTime SendTime { get; private set; }

        public bool IsRead { get; private set; }

        private NotificationEntity(
            string userEmail,
            string title, 
            string message)
        {
            UserEmail = userEmail;
            Title = title;
            Message = message;

            SendTime = DateTime.UtcNow;
            IsRead = false;
        }

        public void Read()
        {
            IsRead = true;
        }

        public static Result<NotificationEntity> Initialize(
            string userEmail,
            string title,
            string message)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if(string.IsNullOrWhiteSpace(userEmail) ||
                !emailRegex.IsMatch(userEmail))
            {
                return Result.Failure<NotificationEntity>("Email is empty or " +
                    "doesnt contains @ or dot after @");
            }

            if(string.IsNullOrWhiteSpace(title) ||
                title.Length < NotificationValidator.MIN_TITLE_LENGTH ||
                title.Length > NotificationValidator.MAX_TITLE_LENGTH)
            {
                return Result.Failure<NotificationEntity>("Title is empty or length out of bounds " +
                    $"{NotificationValidator.MIN_TITLE_LENGTH} - {NotificationValidator.MAX_TITLE_LENGTH}");
            }

            if(string.IsNullOrWhiteSpace(message) ||
                title.Length < NotificationValidator.MIN_MESSAGE_LENGTH ||
                title.Length > NotificationValidator.MAX_MESSAGE_LENGTH)
            {
                return Result.Failure<NotificationEntity>("Message is empty or length out of bounds " +
                    $"{NotificationValidator.MIN_MESSAGE_LENGTH} - {NotificationValidator.MAX_MESSAGE_LENGTH}");
            }

            return Result.Success(new NotificationEntity(
                userEmail,
                title, 
                message));
        }
    }
}
