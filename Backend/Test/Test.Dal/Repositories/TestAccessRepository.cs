using MongoDB.Driver;
using Test.Dal.Persistences;
using Test.Dal.Specifications;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Primitives;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class TestAccessRepository : ITestAccessRepository
    {
        private readonly AppDbContext context;

        public TestAccessRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<TestAccess> AddTestAccess(
            TestAccess testAccess, 
            CancellationToken cancellationToken = default)
        {
            var mongoTestAccess = new MongoTestAccess();
            mongoTestAccess.ConvertToMongoEntity(testAccess);
            var nextId = context.GetNextId(AppDbContext.TEST_ACCESS_COLLECTION_NAME);
            mongoTestAccess.Id = nextId;

            var insertOptions = new InsertOneOptions
            {
                BypassDocumentValidation = true
            };

            await context.Accesses.InsertOneAsync(
                mongoTestAccess, insertOptions, cancellationToken);

            return mongoTestAccess.ConvertToDomainEntity();
        }

        public async Task DeleteTestAccess(
            long testAccessId, 
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<MongoTestAccess>.Filter
                .Eq(x => x.Id, testAccessId);

            await context.Accesses
                .DeleteOneAsync(filter, cancellationToken);
        }

        public async Task<IEnumerable<TestAccess>> GetAccessesByCriteria(
            BaseSpecification<TestAccess> specification, 
            CancellationToken cancellationToken = default)
        {
            var criteria = specification.Criteria is null ?
                Builders<MongoTestAccess>.Filter.Empty :
                new ExpressionConverter<TestAccess, MongoTestAccess>().Rewrite(specification.Criteria);

            var testAccesses = await context.Accesses
                .Find(criteria)
                .ToListAsync(cancellationToken);

            return testAccesses
                .Select(x => x.ConvertToDomainEntity());
        }

        public async Task<TestAccess?> GetTestAccess(
            long testId, 
            long targetEntityId, 
            TargetAccessEntityType entityType,
            CancellationToken cancellationToken = default)
        {
            var testAccess = await context.Accesses
                .Find(x => 
                    x.TestId == testId && 
                    x.TargetEntityId == targetEntityId &&
                    x.TargetAccessEntityType == entityType)
                .FirstOrDefaultAsync(cancellationToken);

            return testAccess?.ConvertToDomainEntity();
        }

        public async Task<TestAccess?> GetTestAccess(
            long accessId, 
            CancellationToken cancellationToken = default)
        {
            var testAccess = await context.Accesses
                .Find(x =>
                    x.Id == accessId)
                .FirstOrDefaultAsync(cancellationToken);

            return testAccess?.ConvertToDomainEntity();
        }
    }
}
