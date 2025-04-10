using Authorization.Domain.Entities;

namespace Authorization.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmail(string email);

        Task<User?> GetUserById(long id);

        Task<User> AddUser(User user);
    }
}
