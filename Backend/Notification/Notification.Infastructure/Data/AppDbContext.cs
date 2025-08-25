using MongoDB.Bson;
using MongoDB.Driver;
using Notification.Infrastructure.Data.Configurations;
using Notification.Infrastructure.Data.Persistences;
using Notification.Infrastructure.Extensions;

namespace Notification.Infrastructure.Data
{
    public class AppDbContext
    {
        public const string NOTIFICATION_COLLECTION_NAME = "notifications";

        private readonly IMongoClient client;
        private readonly IMongoDatabase database;

        private readonly IMongoCollection<BsonDocument> counters;

        public AppDbContext(
            IMongoClient client,
            IMongoDatabase database)
        {
            this.client = client;
            this.database = database;

            counters = database.GetCollection<BsonDocument>("counters");

            Notifications.ApplyConfiguration(new NotificationConfiguration());
        }

        public IMongoClient Client { get => client; }

        public IMongoCollection<MongoNotification> Notifications
        {
            get => database.GetCollection<MongoNotification>(NOTIFICATION_COLLECTION_NAME);
        }

        public long GetTextId(string collectionName)
        {
            var filter = Builders<BsonDocument>.Filter
                .Eq("_id", collectionName);

            var update = Builders<BsonDocument>.Update
                .Inc("seq", 1);

            var option = new FindOneAndUpdateOptions<BsonDocument>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true,
            };

            var result = counters.FindOneAndUpdate(filter, update, option);

            return result["seq"].ToInt64();
        }

        public async Task<long> GetTextIdAsync(string collectionName)
        {
            var filter = Builders<BsonDocument>.Filter
                .Eq("_id", collectionName);

            var update = Builders<BsonDocument>.Update
                .Inc("seq", 1);

            var option = new FindOneAndUpdateOptions<BsonDocument>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true,
            };

            var result = await counters
                .FindOneAndUpdateAsync(filter, update, option);

            return result["seq"].ToInt64();
        }
    }
}
