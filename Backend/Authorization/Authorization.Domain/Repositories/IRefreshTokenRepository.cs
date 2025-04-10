using Authorization.Domain.Entities;

namespace Authorization.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken);

        Task RemoveToken(List<long> refreshTokensList);
    }
}
