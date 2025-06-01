using MongoDB.Driver;
using Notification.Infrastructure.Data.Persistences;

namespace Notification.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration : IConfigurator<MongoNotification>
    {
        public void Configure(IMongoCollection<MongoNotification> collection)
        {
            var indexModel = new CreateIndexModel<MongoNotification>(
                Builders<MongoNotification>.IndexKeys
                .Text(field => field.UserEmail));

            collection.Indexes.CreateOne(indexModel);
        }
    }
}
