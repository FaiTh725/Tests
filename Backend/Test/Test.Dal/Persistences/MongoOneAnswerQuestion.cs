using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;

namespace Test.Dal.Persistences
{
    public class MongoOneAnswerQuestion :
        IMongoPersistence<OneAnswerQuestion, MongoOneAnswerQuestion>
    {
        [BsonId]
        public long Id { get; set; }

        public string TestQuestion { get; set; } = string.Empty;

        public int QuestionWeight { get; set; }

        public long TestId { get; set; }

        public long QuestionAnswerId { get; set; }

        public OneAnswerQuestion ConvertToDomainEntity()
        {
            var questionEntity = OneAnswerQuestion.Initialize(
                TestQuestion, QuestionWeight, TestId, QuestionAnswerId);

            if(questionEntity.IsFailure)
            {
                throw new InvalidDataException("Convert db entity to mongo entity");
            }

            // set Id
            var type = typeof(OneAnswerQuestion);
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(questionEntity.Value, [Id]);

            return questionEntity.Value;
        }

        public MongoOneAnswerQuestion ConvertToMongoEntity(
            OneAnswerQuestion question)
        {
            Id = question.Id;
            TestQuestion = question.TestQuestion;
            QuestionAnswerId = question.QuestionAnswerId;
            QuestionWeight = question.QuestionWeight;
            TestId = question.TestId;
            QuestionAnswerId = question.QuestionAnswerId;

            return this;
        }
    }
}
