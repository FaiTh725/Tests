using Authorization.Domain.Entities;

namespace Authorization.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken = default);

        Task<RefreshToken?> GetRefreshToken(long id, CancellationToken cancellationToken = default);

        Task<RefreshToken?> GetRefreshTokenWithUser(string token, CancellationToken cancellationToken = default);

        Task<RefreshToken?> GetRefreshTokenByUserId(long userId, CancellationToken cancellationToken = default);

        Task RemoveToken(string refreshToken, CancellationToken cancellationToken = default);

        Task UpdateRefreshToken(long id, RefreshToken refreshToken, CancellationToken cancellationToken = default);
    }
}
