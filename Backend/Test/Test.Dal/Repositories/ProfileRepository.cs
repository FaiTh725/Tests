using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Test.Dal.Adapters;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using Test.Domain.Primitives;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext context;

        public ProfileRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<Profile> AddProfile(
            Profile profile, 
            IDatabaseSession? session = null, 
            CancellationToken cancellationToken = default)
        {
            var mongoProfile = new MongoProfile();
            mongoProfile.ConvertToMongoEntity(profile);
            var nextId = context.GetNextId(AppDbContext.PROFILES_COLLECTION_NAME);
            mongoProfile.Id = nextId;

            var insertParameters = new InsertOneOptions
            {
                BypassDocumentValidation = true
            };

            var mongoSession = (session as MongoSessionAdapter)?.Session;
            if (mongoSession is null)
            {
                await context.Profiles.InsertOneAsync(
                mongoProfile,
                insertParameters,
                cancellationToken);
            }
            else
            {
                await context.Profiles.InsertOneAsync(
                    mongoSession,
                    mongoProfile,
                    insertParameters,
                    cancellationToken);
            }

            return mongoProfile.ConvertToDomainEntity();
        }

        public async Task DeleteProfile(
            long id, 
            IDatabaseSession? session = null,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<MongoProfile>.Filter
                .Eq(x => x.Id, id);

            var mongoSession = (session as MongoSessionAdapter)?.Session;
            if (mongoSession is null)
            {
                await context.Profiles.DeleteOneAsync(
                filter,
                cancellationToken: cancellationToken);
            }
            else
            {
                await context.Profiles.DeleteOneAsync(
                    mongoSession,
                    filter,
                    cancellationToken: cancellationToken);
            }
        }

        public async Task<Profile?> GetProfile(long id, CancellationToken cancellationToken = default)
        {
            var mongorProfile =  await context.Profiles
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return mongorProfile?.ConvertToDomainEntity();
        }

        public async Task<Profile?> GetProfile(string email, CancellationToken cancellationToken = default)
        {
            var mongorProfile = await context.Profiles
                .Find(x => x.Email == email)
                .FirstOrDefaultAsync(cancellationToken);

            return mongorProfile?.ConvertToDomainEntity();
        }
    }
}
