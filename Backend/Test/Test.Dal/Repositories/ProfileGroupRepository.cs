using MongoDB.Driver;
using Test.Dal.ExpressionRewriters;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using Test.Domain.Primitives;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class ProfileGroupRepository : IProfileGroupRepository
    {
        private readonly AppDbContext context;

        public ProfileGroupRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ProfileGroup> AddGroup(
            ProfileGroup profileGroup, 
            CancellationToken cancellationToken = default)
        {
            var mongoProfileGroup = new MongoProfileGroup();
            mongoProfileGroup.ConvertToMongoEntity(profileGroup);
            var nextId = context.GetNextId(AppDbContext.GROUPS_COLLECTION_NAME);
            mongoProfileGroup.Id = nextId;

            var insertOptions = new InsertOneOptions
            {
                BypassDocumentValidation = true
            };

            await context.Groups.InsertOneAsync(
                mongoProfileGroup, insertOptions, cancellationToken);

            return mongoProfileGroup.ConvertToDomainEntity();
        }

        public async Task DeleteGroup(
            long groupId, 
            CancellationToken cancellationToken = default)
        {
            await context.Groups
                .DeleteOneAsync(x => x.Id == groupId, cancellationToken);
        }

        public async Task<ProfileGroup?> GetProfileGroup(
            long id, 
            CancellationToken cancellationToken = default)
        {
            var mongoProfileGroup = await context.Groups
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return mongoProfileGroup?.ConvertToDomainEntity();
        }

        public async Task<IEnumerable<ProfileGroup>> GetProfileGroupsByCriteria(
            BaseSpecification<ProfileGroup> specification, 
            CancellationToken cancellationToken = default)
        {
            var filter = specification.Criteria is null ?
                Builders<MongoProfileGroup>.Filter.Empty :
                new ProfileGroupToMongoRewriter().Rewrite(specification.Criteria);

            var groups = await context.Groups
                .Find(filter)
                .ToListAsync(cancellationToken);

            return groups.Select(x => x.ConvertToDomainEntity());
        }

        public async Task UpdateGroup(
            long groupId, 
            ProfileGroup updatedGroup, 
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<MongoProfileGroup>.Filter
                .Eq(x => x.Id, groupId);

            var update = Builders<MongoProfileGroup>.Update
                .Set(x => x.GroupName, updatedGroup.GroupName)
                .Set(x => x.MembersId, [.. updatedGroup.MembersId]);

            await context.Groups.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}
