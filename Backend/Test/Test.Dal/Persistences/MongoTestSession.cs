using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;

namespace Test.Dal.Persistences
{
    public class MongoTestSession : IMongoPersistence<TestSession, MongoTestSession>
    {
        [BsonId]
        public long Id { get; set; }

        public long TestId { get; set; }

        public long ProfileId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public bool IsEnded { get; set; }

        public int Percent {  get; set; }

        public TestSession ConvertToDomainEntity()
        {
            var testSessionEntity = TestSession.Initialize(
                TestId,
                ProfileId);

            if(testSessionEntity.IsFailure)
            {
                throw new InvalidDataException("Error convert db entity to domain entity");
            }

            var type = typeof(TestSession);
            // set Id
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(testSessionEntity.Value, [Id]);

            // set StartTime
            property = type.GetProperty("StartTime");
            setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(testSessionEntity.Value, [StartTime]);

            // set EndTime
            property = type.GetProperty("EndTime");
            setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(testSessionEntity.Value, [EndTime]);

            // Set IsEnded
            property = type.GetProperty("IsEnded");
            setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(testSessionEntity.Value, [IsEnded]);

            // Set Percent
            property = type.GetProperty("Percent");
            setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(testSessionEntity.Value, [Percent]);

            return testSessionEntity.Value;
        }

        public MongoTestSession ConvertToMongoEntity(TestSession testSession)
        {
            Id = testSession.Id;
            TestId = testSession.TestId;
            ProfileId = testSession.ProfileId;
            StartTime = testSession.StartTime;
            EndTime = testSession.EndTime;
            IsEnded = testSession.IsEnded;
            Percent = testSession.Percent;

            return this;
        }
    }
}
