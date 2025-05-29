using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Domain.Repositories
{
    public interface IProfileRepository
    {
        Task<Profile> AddProfile(Profile profile, IDatabaseSession? session = null, CancellationToken cancellationToken = default);

        Task<Profile?> GetProfile(long id, CancellationToken cancellationToken = default);

        Task<Profile?> GetProfile(string email, CancellationToken cancellationToken = default);
    }
}
