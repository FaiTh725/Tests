
using MongoDB.Bson.Serialization.Attributes;
using Test.Domain.Entities;

namespace Test.Dal.Persistences
{
    public class MongoOutboxMessage :
        IMongoPersistence<OutboxMessage, MongoOutboxMessage>
    {
        [BsonId]
        public long Id { get; set; }

        public string Type { get; set; }

        public string Payload { get; set; }

        public DateTime OccurredOnUtc { get; set; }

        public DateTime? ProcessedOnUtc { get; set; }

        public OutboxMessage ConvertToDomainEntity()
        {
            var outboxEntity = OutboxMessage.Initialize(
                Type, Payload);

            if(outboxEntity.IsFailure)
            {
                throw new InvalidDataException("Error convert db entity to domain entity");
            }

            // set Id
            var type = typeof(OutboxMessage);
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(outboxEntity.Value, [Id]);

            // set ProcessedOnUtc
            property = type.GetProperty("OccurredOnUtc");
            setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(outboxEntity.Value, [OccurredOnUtc]);

            // set OccirredOnUtc
            property = type.GetProperty("ProcessedOnUtc");
            setMethod = property!.GetSetMethod(true);
            setMethod?.Invoke(outboxEntity.Value, [ProcessedOnUtc]);

            return outboxEntity.Value;
        }

        public MongoOutboxMessage ConvertToMongoEntity(OutboxMessage outboxMessage)
        {
            Id = outboxMessage.Id;
            Type = outboxMessage.Type;
            Payload = outboxMessage.Payload;
            OccurredOnUtc = outboxMessage.OccurredOnUtc;
            ProcessedOnUtc = outboxMessage.ProcessedOnUtc;

            return this;
        }
    }
}
