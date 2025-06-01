using MongoDB.Driver;
using System.Text.Json;
using Test.Dal.Adapters;
using Test.Dal.Persistences;
using Test.Domain.Interfaces;
using Test.Domain.Primitives;

namespace Test.Dal.Services
{
    public class OutboxService : IOutboxService
    {
        private readonly AppDbContext context;

        public OutboxService(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task AddOutboxMessage<T>(
            T message, 
            IDatabaseSession? session = null,
            CancellationToken cancellationToken = default)
            where T : notnull
        {
            var mongoOutboxMessage = new MongoOutboxMessage
            {
                Id = context.GetNextId(AppDbContext.OUTBOX_MESSAGE_COLLECTION_NAME),
                OccurredOnUtc = DateTime.UtcNow,
                Type = message.GetType().AssemblyQualifiedName!,
                Payload = JsonSerializer.Serialize(message)
            };

            var incertOneOptions = new InsertOneOptions
            {
                BypassDocumentValidation = true
            };

            var mongoSession = (session as MongoSessionAdapter)?.Session;
            if (mongoSession is null)
            {
                await context.OutboxMessages.InsertOneAsync(
                    mongoOutboxMessage, 
                    incertOneOptions, 
                    cancellationToken);
            }
            else
            {
                await context.OutboxMessages.InsertOneAsync(
                    mongoSession,
                    mongoOutboxMessage,
                    incertOneOptions,
                    cancellationToken);
            }
        }
    }
}
