using MongoDB.Driver;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class OutboxMessageRepository : IOutboxMessageRepository
    {
        private readonly AppDbContext context;

        public OutboxMessageRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<OutboxMessage>> GetPendingMessages(
            CancellationToken cancellation = default)
        {
            var filter = Builders<MongoOutboxMessage>.Filter
                .Eq(x => x.ProcessedOnUtc, null);

            var pendingMessages = await context.OutboxMessages
                .Find(filter)
                .ToListAsync(cancellation);

            return pendingMessages.Select(x => x.ConvertToDomainEntity());
        }

        public async Task UpdateMessage(
            long id, 
            OutboxMessage updatedMessage, 
            CancellationToken cancellationToken = default)
        {
            var mongoOutboxMessage = new MongoOutboxMessage();
            mongoOutboxMessage.ConvertToMongoEntity(updatedMessage);

            var filter = Builders<MongoOutboxMessage>.Filter
                .Eq(x => x.Id, id);

            var update = Builders<MongoOutboxMessage>.Update
                .Set(x => x.ProcessedOnUtc, mongoOutboxMessage.ProcessedOnUtc);

            if (context.Session is null)
            {
                await context.OutboxMessages
                    .UpdateOneAsync(
                    filter, 
                    update, 
                    cancellationToken: cancellationToken);
            }
            else
            {
                await context.OutboxMessages
                    .UpdateOneAsync(
                    context.Session,
                    filter,
                    update,
                    cancellationToken: cancellationToken);
            }
        }
    }
}
