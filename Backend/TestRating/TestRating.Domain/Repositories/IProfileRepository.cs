using TestRating.Domain.Entities;

namespace TestRating.Domain.Repositories
{
    public interface IProfileRepository
    {
        Task<Profile> AddProfile(Profile profile, CancellationToken cancellationToken = default);

        Task<Profile?> GetProfileByEmail(string email, CancellationToken cancellationToken = default);

        Task<Profile?> GetProfileById(long id, CancellationToken cancellationToken = default);

        Task DeleteProfile(long id, CancellationToken cancellationToken = default);
    }
}
