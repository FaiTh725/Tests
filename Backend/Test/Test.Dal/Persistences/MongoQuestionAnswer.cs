using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;

namespace Test.Dal.Persistences
{
    public class MongoQuestionAnswer : IMongoPersistence<QuestionAnswer, MongoQuestionAnswer>
    {
        [BsonId]
        public long Id { get; set; }

        public string Answer { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public long QuestionId { get; set; }

        public QuestionAnswer ConvertToDomainEntity()
        {
            var questionAnswerEntity = QuestionAnswer.Initialize(
                Answer, IsCorrect, QuestionId);

            if (questionAnswerEntity.IsFailure)
            {
                throw new InvalidDataException("Convert db entity to mongo entity");
            }

            // set Id
            var type = typeof(QuestionAnswer);
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod!.Invoke(questionAnswerEntity.Value, [Id]);

            return questionAnswerEntity.Value;
        }

        public MongoQuestionAnswer ConvertToMongoEntity(
            QuestionAnswer questionAnswer)
        {
            Id = questionAnswer.Id;
            Answer = questionAnswer.Answer;
            IsCorrect = questionAnswer.IsCorrect;
            QuestionId = questionAnswer.QuestionId;

            return this;
        }
    }
}
