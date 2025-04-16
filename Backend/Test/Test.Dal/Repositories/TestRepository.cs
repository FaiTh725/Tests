using MongoDB.Driver;
using Test.Dal.Persistences;
using Test.Domain.Repositories;
using TestEntity = Test.Domain.Entities.Test;

namespace Test.Dal.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly AppDbContext context;

        public TestRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<TestEntity> AddTest(
            TestEntity test, CancellationToken cancellationToken = default)
        {
            var mongoTest = new MongoTest();
            mongoTest.ConvertToMongoEntity(test);
            var nextId = context.GetNextId(AppDbContext.TESTS_COLLECTION_NAME);
            mongoTest.Id = nextId;

            var insertOptions = new InsertOneOptions
            {
                BypassDocumentValidation = true
            };

            await context.Tests.InsertOneAsync(
                mongoTest, insertOptions, cancellationToken);

            return mongoTest.ConvertToDomainEntity();
        }

        public async Task<TestEntity?> GetTest(
            long id, CancellationToken cancellationToken = default)
        {
            var mongoTest = await context.Tests
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return mongoTest?.ConvertToDomainEntity();
        }
    }
}
