using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;

namespace Test.Dal.Persistences
{
    public class MongoQuestionAnswer : IMongoPersistence<QuestionAnswer, MongoQuestionAnswer>
    {
        [BsonId]
        public long Id { get; set; }

        [BsonIgnore]
        public string ImageFolder { get => $"Answer-{Id}"; }

        public string Answer { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public long QuestionId { get; set; }

        public QuestionAnswer ConvertToDomainEntity()
        {
            throw new NotImplementedException();
        }

        public MongoQuestionAnswer ConvertToMongoEntity(QuestionAnswer entity)
        {
            throw new NotImplementedException();
        }
    }
}
