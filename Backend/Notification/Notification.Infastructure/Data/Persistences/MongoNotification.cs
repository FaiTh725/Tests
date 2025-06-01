using MongoDB.Bson.Serialization.Attributes;
using Notification.Domain.Entities;

namespace Notification.Infrastructure.Data.Persistences
{
    public class MongoNotification : IMongoPersistence<NotificationEntity, MongoNotification>
    {
        [BsonId]
        public long Id { get; set; }

        public string UserEmail { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime SendTime { get; set; }

        public bool IsRead { get; set; }

        public NotificationEntity ConvertToDomainEntity()
        {
            var notification = NotificationEntity.Initialize(
                UserEmail, Title, Message);

            if(notification.IsFailure)
            {
                throw new InvalidDataException("Error convert db entity to mongo entity");
            }

            // set Id
            var type = typeof(NotificationEntity);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(notification.Value, [Id]);

            // set SendTime
            type = typeof(NotificationEntity);
            property = type.GetProperty("SendTime");
            method = property!.GetSetMethod(true);
            method!.Invoke(notification.Value, [SendTime]);

            // set IsRead
            type = typeof(NotificationEntity);
            property = type.GetProperty("IsRead");
            method = property!.GetSetMethod(true);
            method!.Invoke(notification.Value, [IsRead]);

            return notification.Value;
        }

        public MongoNotification ConvertToMongoEntity(
            NotificationEntity notification)
        {
            Id = notification.Id;
            UserEmail = notification.UserEmail;
            Title = notification.Title;
            Message = notification.Message;
            SendTime = notification.SendTime;
            IsRead = notification.IsRead;

            return this;
        }
    }
}
