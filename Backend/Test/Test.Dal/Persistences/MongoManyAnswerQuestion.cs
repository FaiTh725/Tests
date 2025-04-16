using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;

namespace Test.Dal.Persistences
{
    public class MongoManyAnswerQuestion :
        IMongoPersistence<ManyAnswersQuestion, MongoManyAnswerQuestion>
    {
        [BsonId]
        public long Id { get; set; }

        [BsonIgnore]
        public string ImageFolder { get => $"Question-{Id}"; }

        public string TestQuestion { get; set; } = string.Empty;

        public int QuestionWeight { get; set; }

        public List<long> QuestionAnswerId { get; set; } = new List<long>();

        public ManyAnswersQuestion ConvertToDomainEntity()
        {
            throw new NotImplementedException();
        }

        public MongoManyAnswerQuestion ConvertToMongoEntity(ManyAnswersQuestion entity)
        {
            throw new NotImplementedException();
        }
    }
}
