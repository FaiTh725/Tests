using MongoDB.Driver;
using Test.Dal.Persistences;

namespace Test.Dal.Configurations
{
    public class TestAccessConfiguration
    {
        public static void ApplyTestAccessConfigurations(IMongoCollection<MongoTestAccess> collection)
        {
            var testAccessIndex = new CreateIndexModel<MongoTestAccess>(
                Builders<MongoTestAccess>.IndexKeys
                .Ascending(x => x.TestId)
                .Ascending(x => x.TargetEntityId));

            collection.Indexes.CreateOne(testAccessIndex);
        }
    }
}
