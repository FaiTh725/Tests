using Authorization.Domain.Entities;
using Authorization.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Dal.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext context;

        public RoleRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<Role> AddRole(Role role)
        {
            var roleEntity = await context.Roles
                .AddAsync(role);

            return roleEntity.Entity;
        }

        public async Task<Role?> GetRole(string roleName)
        {
            return await context.Roles
                .FirstOrDefaultAsync(x => x.RoleName == roleName);
        }
    }
}
