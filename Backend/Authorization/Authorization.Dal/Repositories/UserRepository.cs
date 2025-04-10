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

        public async Task<User> AddUser(User user)
        {
            var userEntity = await context.Users.AddAsync(user);

            return userEntity.Entity;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetUserById(long id)
        {
            return await context.Users
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
