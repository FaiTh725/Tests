using MongoDB.Driver;

namespace Notification.Infrastructure.Data.Configurations
{
    public interface IConfigurator<MongoEntity>
    {
        void Configure(IMongoCollection<MongoEntity> collection);
    }
}
