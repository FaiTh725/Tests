using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;

namespace Test.Dal.Persistences
{
    public class MongoProfileAnswer : IMongoPersistence<ProfileAnswer, MongoProfileAnswer>
    {
        [BsonId]
        public long Id { get; set; }

        public long SessionId { get; set; }

        public long QuestionId { get; set; }

        public List<long> QuestionAnswersId { get; set; } = new List<long>();

        public DateTime SendTime { get; set; }

        public bool IsCorrect { get; set; }

        public ProfileAnswer ConvertToDomainEntity()
        {
            var profileAnswerEntity = ProfileAnswer.Initialize(
                SessionId,
                QuestionId,
                QuestionAnswersId,
                IsCorrect);

            if(profileAnswerEntity.IsFailure)
            {
                throw new InvalidDataException("Error convert db entity to domain entity");
            }

            var type = typeof(ProfileAnswer);
            // set Id
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(profileAnswerEntity.Value, [Id]);

            // set SendTime
            property = type.GetProperty("SendTime");
            setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(profileAnswerEntity.Value, [SendTime]);

            return profileAnswerEntity.Value;
        }

        public MongoProfileAnswer ConvertToMongoEntity(ProfileAnswer profileAnswer)
        {
            Id = profileAnswer.Id; 
            SessionId = profileAnswer.SessionId; 
            QuestionId = profileAnswer.QuestionId; 
            QuestionAnswersId = profileAnswer.QuestionAnswersId;
            SendTime = profileAnswer.SendTime;
            IsCorrect = profileAnswer.IsCorrect;

            return this;
        }
    }
}
