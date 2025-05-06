using CSharpFunctionalExtensions;

namespace Test.Domain.Entities
{
    public class OutboxMessage : Entity
    {
        public string Type { get; private set; }

        public string Payload { get; private set; }

        public DateTime OccurredOnUtc { get; private set; }

        public DateTime? ProcessedOnUtc { get; private set; }

        private OutboxMessage(
            string type,
            string payload) 
        { 
            Type = type; 
            Payload = payload;
        
            OccurredOnUtc = DateTime.UtcNow;
        }

        public void ProcessMessage()
        {
            ProcessedOnUtc = DateTime.UtcNow;
        }

        public static Result<OutboxMessage> Initialize(
            string type,
            string payload)
        {
            if(string.IsNullOrEmpty(type))
            {
                return Result.Failure<OutboxMessage>("Type is empty");
            }

            if(string.IsNullOrEmpty(payload))
            {
                return Result.Failure<OutboxMessage>("Payload is empty");
            }

            return Result.Success(new OutboxMessage(
                type, payload));
        }
    }
}
