using Authorization.Domain.Entities;

namespace Authorization.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetRole(string roleName, CancellationToken cancellationToken = default);

        Task<Role> AddRole(Role role, CancellationToken cancellationToken = default);

        Task<IEnumerable<Role>> GetRoles(CancellationToken cancellationToken = default);
    }
}
