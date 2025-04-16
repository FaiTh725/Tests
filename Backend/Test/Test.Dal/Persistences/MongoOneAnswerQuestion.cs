using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;

namespace Test.Dal.Persistences
{
    public class MongoOneAnswerQuestion :
        IMongoPersistence<OneAnswerQuestion, MongoOneAnswerQuestion>
    {
        [BsonId]
        public long Id { get; set; }

        [BsonIgnore]
        public string ImageFolder { get => $"Question-{Id}"; }

        public string TestQuestion { get; set; } = string.Empty;

        public int QuestionWeight { get; set; }

        public long QuestionAnswerId { get; set; }

        public OneAnswerQuestion ConvertToDomainEntity()
        {
            throw new NotImplementedException();
        }

        public MongoOneAnswerQuestion ConvertToMongoEntity(OneAnswerQuestion entity)
        {
            throw new NotImplementedException();
        }
    }
}
