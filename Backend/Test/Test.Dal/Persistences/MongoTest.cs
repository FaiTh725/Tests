using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Enums;
using TestEntity = Test.Domain.Entities.Test;

namespace Test.Dal.Persistences
{
    public class MongoTest : IMongoPersistence<TestEntity, MongoTest>
    {
        [BsonId]
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedTime { get; set; }

        public bool IsPublic { get; set; }

        public TestType TestType { get; set; }

        public long ProfileId { get; private set; }

        public List<long> QuestionsId { get; set; } = new List<long>();

        public TestEntity ConvertToDomainEntity()
        {
            throw new NotImplementedException();
        }

        public MongoTest ConvertToMongoEntity(TestEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
