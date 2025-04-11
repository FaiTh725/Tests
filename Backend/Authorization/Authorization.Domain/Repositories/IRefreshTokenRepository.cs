using Authorization.Domain.Entities;

namespace Authorization.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken);

        Task<RefreshToken?> GetRefreshToken(long id);

        Task<RefreshToken?> GetRefreshTokenWithUser(string token);

        Task RemoveToken(string refreshToken);

        Task UpdateRefreshToken(long id, RefreshToken refreshToken);
    }
}
