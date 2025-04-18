using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;
using Test.Domain.Enums;

namespace Test.Dal.Persistences
{
    public class MongoQuestion : IMongoPersistence<Question, MongoQuestion>
    {
        [BsonId]
        public long Id { get; set; }

        public string TestQuestion { get; set; } = string.Empty;

        public int QuestionWeight { get; set; }

        public QuestionType QuestionType { get; set; }

        public long TestId { get; set; }

        public Question ConvertToDomainEntity()
        {
            var questionEntity = Question.Initialize(
                TestQuestion, QuestionWeight, 
                QuestionType, TestId);

            if (questionEntity.IsFailure)
            {
                throw new InvalidDataException("Convert from db entity to mongo entity");
            }

            // set Id
            var type = typeof(Question);
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(questionEntity.Value, [Id]);

            return questionEntity.Value;
        }

        public MongoQuestion ConvertToMongoEntity(Question question)
        {
            Id = question.Id;
            TestQuestion = question.TestQuestion;
            QuestionWeight = question.QuestionWeight;
            QuestionType = question.QuestionType;
            TestId = question.TestId;

            return this;
        }
    }
}
