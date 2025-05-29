using MongoDB.Driver;
using Test.Dal.Persistences;

namespace Test.Dal.Configurations
{
    public class ProfileConfiguration
    {
        public static void ApplyProfileConfigurations(IMongoCollection<MongoProfile> collection)
        {
            var profileEmailIndex = new CreateIndexModel<MongoProfile>(
                Builders<MongoProfile>.IndexKeys.Ascending(x => x.Email));

            collection.Indexes.CreateOne(profileEmailIndex);
        }
    }
}
