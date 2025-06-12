using Microsoft.EntityFrameworkCore;
using TestRating.Domain.Entities;
using TestRating.Domain.Repositories;

namespace TestRating.Dal.Repositories
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
            CancellationToken cancellationToken = default)
        {
            var profileEntity = await context.Profiles
                .AddAsync(profile, cancellationToken);

            return profileEntity.Entity;
        }

        public async Task DeleteProfile(
            long id, 
            CancellationToken cancellationToken = default)
        {
            await context.Profiles
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<Profile?> GetProfileByEmail(
            string email, 
            CancellationToken cancellationToken = default)
        {
            return await context.Profiles
                .FirstOrDefaultAsync(x => x.Email == email, 
                cancellationToken);
        }

        public async Task<Profile?> GetProfileById(
            long id, 
            CancellationToken cancellationToken = default)
        {
            return await context.Profiles
                .FirstOrDefaultAsync(x => x.Id == id, 
                cancellationToken);
        }
    }
}
