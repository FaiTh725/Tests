using Authorization.Domain.Entities;

namespace Authorization.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken = default);

        Task<User?> GetUserById(long id, CancellationToken cancellationToken = default);

        Task<User> AddUser(User user, CancellationToken cancellationToken = default);
    }
}
