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

        public string TestQuestion { get; set; } = string.Empty;

        public long TestId { get; set; }

        public int QuestionWeight { get; set; }

        public List<long> QuestionAnswerId { get; set; } = new List<long>();

        public ManyAnswersQuestion ConvertToDomainEntity()
        {
            var questionEntity = ManyAnswersQuestion.Initialize(
                TestQuestion, QuestionWeight, TestId, QuestionAnswerId);

            if(questionEntity.IsFailure)
            {
                throw new InvalidDataException("Convert db entity to mongo entity");
            }

            // set Id
            var type = typeof(ManyAnswersQuestion);
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod!.Invoke(questionEntity.Value, [Id]);

            return questionEntity.Value;
        }

        public MongoManyAnswerQuestion ConvertToMongoEntity(
            ManyAnswersQuestion question)
        {
            Id = question.Id;
            QuestionWeight = question.QuestionWeight;
            TestQuestion = question.TestQuestion;
            TestId = question.TestId;
            QuestionAnswerId = [.. question.QuestionAnswerId];

            return this;
        }
    }
}
