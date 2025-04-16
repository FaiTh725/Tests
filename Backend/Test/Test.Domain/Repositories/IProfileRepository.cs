using Test.Domain.Entities;

namespace Test.Domain.Repositories
{
    public interface IProfileRepository
    {
        Task<Profile> AddProfile(Profile profile, CancellationToken cancellationToken = default);

        Task<Profile?> GetProfile(long id, CancellationToken cancellationToken = default);

        Task<Profile?> GetProfile(string email, CancellationToken cancellationToken = default);
    }
}
