﻿using MongoDB.Bson;
using MongoDB.Driver;
using Test.Dal.Persistences;
using Test.Domain.Entities;
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

        public async Task<Profile> AddProfile(Profile profile, CancellationToken cancellationToken = default)
        {
            var mongoProfile = new MongoProfile();
            mongoProfile.ConvertToMongoEntity(profile);
            var nextId = context.GetNextId(AppDbContext.PROFILES_COLLECTION_NAME);
            mongoProfile.Id = nextId;

            var insertParameters = new InsertOneOptions
            {
                BypassDocumentValidation = true
            };

            await context.Profiles.InsertOneAsync(
                mongoProfile, 
                insertParameters,
                cancellationToken);

            return mongoProfile.ConvertToDomainEntity();
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
