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

        public async Task<Role> AddRole(
            Role role, CancellationToken cancellationToken = default)
        {
            var roleEntity = await context.Roles
                .AddAsync(role, cancellationToken);

            return roleEntity.Entity;
        }

        public async Task<Role?> GetRole(
            string roleName, CancellationToken cancellationToken = default)
        {
            return await context.Roles
                .FirstOrDefaultAsync(x => x.RoleName == roleName, 
                cancellationToken);
        }

        public async Task<IEnumerable<Role>> GetRoles(CancellationToken cancellationToken = default)
        {
            return await context.Roles
                .ToListAsync(cancellationToken);
        }
    }
}
