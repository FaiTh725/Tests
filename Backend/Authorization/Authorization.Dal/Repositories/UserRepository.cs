using Authorization.Domain.Entities;
using Authorization.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Dal.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;

        public UserRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<User> AddUser(
            User user, CancellationToken cancellationToken = default)
        {
            var userEntity = await context.Users
                .AddAsync(user, cancellationToken);

            return userEntity.Entity;
        }

        public async Task<User?> GetUserByEmail(
            string email, CancellationToken cancellationToken = default)
        {
            return await context.Users
                .FirstOrDefaultAsync(x => x.Email == email, 
                cancellationToken);
        }

        public async Task<User?> GetUserById(
            long id, CancellationToken cancellationToken = default)
        {
            return await context.Users
                .FirstOrDefaultAsync(x => x.Id == id, 
                cancellationToken);
        }
    }
}
