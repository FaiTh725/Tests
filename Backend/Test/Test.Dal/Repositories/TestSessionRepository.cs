using MongoDB.Driver;
using Test.Dal.Adapters;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using Test.Domain.Primitives;
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
            IDatabaseSession? session = null,
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

            var mongoSession = (session as MongoSessionAdapter)?.Session;
            if (mongoSession is null)
            {
                await context.Sessions.InsertOneAsync(
                    mongoTestSession,
                    insertOptions,
                    cancellationToken);
            }
            else
            {
                await context.Sessions.InsertOneAsync(
                    mongoSession,
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
            IDatabaseSession? session = null,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<MongoTestSession>.Filter
                .Eq(x => x.Id, id);

            var update = Builders<MongoTestSession>.Update
                .Set(x => x.EndTime, updatedSession.EndTime)
                .Set(x => x.IsEnded, updatedSession.IsEnded)
                .Set(x => x.Percent, updatedSession.Percent);

            var mongoSession = (session as MongoSessionAdapter)?.Session;
            if (mongoSession is null)
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
                    mongoSession,
                    filter,
                    update,
                    cancellationToken: cancellationToken);
            }
        }
    }
}
