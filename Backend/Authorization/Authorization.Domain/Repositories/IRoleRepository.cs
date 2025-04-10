using Authorization.Domain.Entities;

namespace Authorization.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetRole(string roleName);

        Task<Role> AddRole(Role role);
    }
}
