
using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Domain.Repositories
{
    public interface IProfileGroupRepository
    {
        Task<ProfileGroup> AddGroup(ProfileGroup profileGroup, IDatabaseSession? session = null, CancellationToken cancellationToken = default);

        Task<ProfileGroup?> GetProfileGroup(long id, CancellationToken cancellationToken = default);

        Task<IEnumerable<ProfileGroup>> GetProfileGroupsByCriteria(
            BaseSpecification<ProfileGroup> specification, 
            CancellationToken cancellationToken = default);

        Task UpdateGroup(long groupId, ProfileGroup updatedGroup, IDatabaseSession? session = null, CancellationToken cancellationToken = default);

        Task DeleteGroup(long groupId, IDatabaseSession? session = null, CancellationToken cancellationToken = default);
    }
}
