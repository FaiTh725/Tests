using CSharpFunctionalExtensions;
using Notification.Domain.Primitives;
using Notification.Domain.Validators;
using System.Text.RegularExpressions;

namespace Notification.Domain.Entities
{
    public class Notification : BaseEntity
    {
        // if use user id then id from other services may be not unique
        public string UserEmail { get; private set; }

        public string Title { get; private set; }

        public string Message { get; private set; }

        public DateTime SendTime { get; private set; }

        public bool IsRead { get; private set; }

        private Notification(
            string userEmail,
            string title, 
            string message)
        {
            Title = title;
            Message = message;

            SendTime = DateTime.UtcNow;
            IsRead = false;
        }

        public void Read()
        {
            IsRead = true;
        }

        public static Result<Notification> Initialize(
            string userEmail,
            string title,
            string message)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if(string.IsNullOrWhiteSpace(userEmail) ||
                !emailRegex.IsMatch(userEmail))
            {
                return Result.Failure<Notification>("Email is empty or " +
                    "doesnt contains @ or dot after @");
            }

            if(string.IsNullOrWhiteSpace(title) ||
                title.Length < NotificationValidator.MIN_TITLE_LENGTH ||
                title.Length > NotificationValidator.MAX_TITLE_LENGTH)
            {
                return Result.Failure<Notification>("Title is empty or length out of bounds " +
                    $"{NotificationValidator.MIN_TITLE_LENGTH} - {NotificationValidator.MAX_TITLE_LENGTH}");
            }

            if(string.IsNullOrWhiteSpace(message) ||
                title.Length < NotificationValidator.MIN_MESSAGE_LENGTH ||
                title.Length > NotificationValidator.MAX_MESSAGE_LENGTH)
            {
                return Result.Failure<Notification>("Message is empty or length out of bounds " +
                    $"{NotificationValidator.MIN_MESSAGE_LENGTH} - {NotificationValidator.MAX_MESSAGE_LENGTH}");
            }

            return Result.Success(new Notification(
                userEmail,
                title, 
                message));
        }
    }
}
