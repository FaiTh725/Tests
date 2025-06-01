using MongoDB.Driver;
using Notification.Infrastructure.Data.Configurations;

namespace Notification.Infrastructure.Extensions
{
    public static class MongoExtensions
    {
        public static IMongoCollection<TMongoEntity> ApplyConfiguration<TMongoEntity>(
            this IMongoCollection<TMongoEntity> collection,
            IConfigurator<TMongoEntity> configurator)
        {
            configurator.Configure(collection);

            return collection;
        }
    }
}
