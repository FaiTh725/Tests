using MongoDB.Driver;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class TestSessionRepository : ITestSessionRepository
    {
        private readonly AppDbContext context;

        public TestSessionRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<TestSession> AddTestSession(
            TestSession testSession, 
            CancellationToken cancellationToken = default)
        {
            var mongoTestSession = new MongoTestSession();
            mongoTestSession.ConvertToMongoEntity(testSession);
            var nextId = context.GetNextId(AppDbContext.TEST_SESSIONS_COLLECTION_NAME);
            mongoTestSession.Id = nextId;

            var insertOptions = new InsertOneOptions
            {
                BypassDocumentValidation = true
            };

            if (context.Session is null)
            {
                await context.Sessions.InsertOneAsync(
                    mongoTestSession,
                    insertOptions,
                    cancellationToken);
            }
            else
            {
                await context.Sessions.InsertOneAsync(
                    context.Session,
                    mongoTestSession,
                    insertOptions,
                    cancellationToken);
            }

            return mongoTestSession.ConvertToDomainEntity();
        }

        public async Task<TestSession?> GetTestSession(
            long testSessionId, 
            CancellationToken cancellationToken = default)
        {
            var mongoTestSession = await context.Sessions
                .Find(x => x.Id == testSessionId)
                .FirstOrDefaultAsync(cancellationToken);

            return mongoTestSession?.ConvertToDomainEntity();
        }

        public async Task UpdateTestSession(
            long id, 
            TestSession updatedSession, 
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<MongoTestSession>.Filter
                .Eq(x => x.Id, id);

            var update = Builders<MongoTestSession>.Update
                .Set(x => x.EndTime, updatedSession.EndTime)
                .Set(x => x.IsEnded, updatedSession.IsEnded)
                .Set(x => x.Percent, updatedSession.Percent);

            if (context.Session is null)
            {
                await context.Sessions
                    .UpdateOneAsync(
                    filter,
                    update,
                    cancellationToken: cancellationToken);
            }
            else
            {
                await context.Sessions
                    .UpdateOneAsync(
                    context.Session,
                    filter,
                    update,
                    cancellationToken: cancellationToken);
            }
        }
    }
}
