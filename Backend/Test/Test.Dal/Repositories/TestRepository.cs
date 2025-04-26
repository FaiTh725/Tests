using MongoDB.Driver;
using Test.Dal.ExpressionRewriters;
using Test.Dal.Persistences;
using Test.Domain.Primitives;
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

        public async Task DeleteTest(long id, CancellationToken cancellationToken = default)
        {
            await context.Tests.DeleteOneAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<TestEntity?> GetTest(
            long id, CancellationToken cancellationToken = default)
        {
            var mongoTest = await context.Tests
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return mongoTest?.ConvertToDomainEntity();
        }

        public async Task<TestEntity?> GetTestByCriteria(
            BaseSpecification<TestEntity> specification, 
            CancellationToken cancellationToken)
        {
            var filter = specification.Criteria is null ?
                Builders<MongoTest>.Filter.Empty :
                new TestToMongoRewriter().Rewrite(specification.Criteria);

            var test = await context.Tests
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);

            return test?.ConvertToDomainEntity();
        }

        public async Task<IEnumerable<TestEntity>> GetTestsByCriteria(
            BaseSpecification<TestEntity> specification, 
            CancellationToken cancellationToken)
        {
            var filter = specification.Criteria is null ?
                Builders<MongoTest>.Filter.Empty :
                new TestToMongoRewriter().Rewrite(specification.Criteria);

            var tests = await context.Tests
                .Find(filter)
                .ToListAsync(cancellationToken);

            return tests.Select(x => x.ConvertToDomainEntity());
        }

        public async Task UpdateTest(long id, TestEntity updatedTest, 
            CancellationToken cancellationToken = default)
        {
            var mongoTest = new MongoTest();
            mongoTest.ConvertToMongoEntity(updatedTest);

            var filter = Builders<MongoTest>.Filter
                .Eq(x => x.Id, id);

            var update = Builders<MongoTest>.Update
                .Set(x => x.Name, mongoTest.Name)
                .Set(x => x.Description, mongoTest.Description)
                .Set(x => x.IsPublic, mongoTest.IsPublic)
                .Set(x => x.TestType, mongoTest.TestType)
                .Set(x => x.DurationInMinutes, mongoTest.DurationInMinutes);
                
            await context.Tests
                .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}
