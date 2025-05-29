using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;
using Test.Domain.Enums;

namespace Test.Dal.Persistences
{
    public class MongoTestAccess : IMongoPersistence<TestAccess, MongoTestAccess>
    {
        [BsonId]
        public long Id { get; set; }

        public long TestId { get; set; }

        public long TargetEntityId { get; set; }

        public TargetAccessEntityType TargetAccessEntityType { get; set; }

        public TestAccess ConvertToDomainEntity()
        {
            var testAccessEntity = TestAccess.Initialize(
                TestId,
                TargetEntityId,
                TargetAccessEntityType);

            if(testAccessEntity.IsFailure)
            {
                throw new InvalidDataException("Error convert db entity to domain entity");
            }

            // set Id
            var type = typeof(TestAccess);
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(testAccessEntity.Value, [Id]);

            return testAccessEntity.Value;
        }

        public MongoTestAccess ConvertToMongoEntity(TestAccess testAccess)
        {
            TestId = testAccess.TestId;
            TargetEntityId = testAccess.TargetEntityId;
            Id = testAccess.Id;
            TargetAccessEntityType = testAccess.TargetAccessEntityType;

            return this;
        }
    }
}
