using MongoDB.Driver;
using Notification.Domain.Entities;
using Notification.Domain.Primitives;
using Notification.Domain.Repositories;
using Notification.Infrastructure.Data.Adapters;
using Notification.Infrastructure.Data.Persistences;
using Notification.Infrastructure.Data.Specification;

namespace Notification.Infrastructure.Data.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext context;

        public NotificationRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<NotificationEntity> AddNotification(
            NotificationEntity notification,
            IDatabaseTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            var mongoNotification = new MongoNotification();
            mongoNotification.ConvertToMongoEntity(notification);
            var nextId = await context
                .GetTextIdAsync(AppDbContext.NOTIFICATION_COLLECTION_NAME);
            mongoNotification.Id = nextId;

            var insertOptions = new InsertOneOptions
            {
                BypassDocumentValidation = true
            };

            var mongoTransaction = (transaction as MongoTransactionAdapter)?.Session;
            if(mongoTransaction is null)
            {
                await context.Notifications.InsertOneAsync(
                    mongoNotification, 
                    insertOptions, 
                    cancellationToken);
            }
            else
            {
                await context.Notifications.InsertOneAsync(
                    mongoTransaction,
                    mongoNotification,
                    insertOptions,
                    cancellationToken);
            }

            return mongoNotification.ConvertToDomainEntity();
        }

        public async Task DeleteNotifications(
            List<long> notificationsIdToDelete,
            IDatabaseTransaction? databaseTransaction = null,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<MongoNotification>.Filter
                .In(x => x.Id, notificationsIdToDelete);

            var mongoTransaction = (databaseTransaction as MongoTransactionAdapter)?.Session;

            if(mongoTransaction is null)
            {
                await context.Notifications
                    .DeleteManyAsync(
                    filter, 
                    cancellationToken);
            }
            else
            {
                await context.Notifications
                    .DeleteManyAsync(
                    mongoTransaction,
                    filter,
                    cancellationToken: cancellationToken);
            }
        }

        public async Task<NotificationEntity?> GetNotification(
            long notificationId, 
            CancellationToken cancellationToken = default)
        {
            var mongoNotification = await context.Notifications
                .Find(x => x.Id == notificationId)
                .FirstOrDefaultAsync(cancellationToken);

            return mongoNotification?.ConvertToDomainEntity();
        }

        public async Task<IEnumerable<NotificationEntity>> GetNotifications(
            CancellationToken cancellationToken = default)
        {
            var mongoNotifications = await context.Notifications
                .Find(FilterDefinition<MongoNotification>.Empty)
                .ToListAsync(cancellationToken);

            var notifications = mongoNotifications
                .Select(x => x.ConvertToDomainEntity());

            return notifications;
        }

        public async Task<IEnumerable<NotificationEntity>> GetNotificationsByCriteria(
            BaseSpecification<NotificationEntity> specification, 
            CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator
                .GetQueryAsync(
                    context.Notifications, 
                    specification, 
                    cancellationToken);
        }

        public async Task MarkUserNotificationsAsRead(
            string userEmail,
            IDatabaseTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<MongoNotification>.Filter
                .Eq(x => x.UserEmail, userEmail);

            var update = Builders<MongoNotification>.Update
                .Set(x => x.IsRead, true);

            var mongoTransaction = (transaction as MongoTransactionAdapter)?.Session;
            if(mongoTransaction is null)
            {
                await context.Notifications.UpdateManyAsync(
                    filter, 
                    update, 
                    cancellationToken: cancellationToken);
            }
            else
            {
                await context.Notifications.UpdateManyAsync(
                    mongoTransaction,
                    filter,
                    update,
                    cancellationToken: cancellationToken);
            }
        }

        public async Task UpdateNotification(
            long notificationIdToUpdate, 
            NotificationEntity updatedNotification,
            IDatabaseTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<MongoNotification>.Filter
                .Eq(x => x.Id, notificationIdToUpdate);

            var update = Builders<MongoNotification>.Update
                .Set(x => x.IsRead, updatedNotification.IsRead);

            var mongoTransaction = (transaction as MongoTransactionAdapter)?.Session;
            if(mongoTransaction is null)
            {
                await context.Notifications
                    .UpdateOneAsync(
                        filter, update, 
                        cancellationToken: cancellationToken);
            }
            else
            {
                await context.Notifications
                    .UpdateOneAsync(
                        mongoTransaction, 
                        filter, update, 
                        cancellationToken: cancellationToken);
            }
        }
    }
}
