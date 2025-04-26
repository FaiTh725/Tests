
using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Domain.Repositories
{
    public interface IProfileGroupRepository
    {
        Task<ProfileGroup> AddGroup(ProfileGroup profileGroup, CancellationToken cancellationToken = default);

        Task<ProfileGroup?> GetProfileGroup(long id, CancellationToken cancellationToken = default);

        Task<IEnumerable<ProfileGroup>> GetProfileGroupsByCriteria(
            BaseSpecification<ProfileGroup> specification, 
            CancellationToken cancellationToken = default);

        Task UpdateGroup(long groupId, ProfileGroup updatedGroup, CancellationToken cancellationToken = default);

        Task DeleteGroup(long groupId, CancellationToken cancellationToken = default);
    }
}
