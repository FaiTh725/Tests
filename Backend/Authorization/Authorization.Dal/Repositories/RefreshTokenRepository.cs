using Authorization.Domain.Entities;
using Authorization.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Dal.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext context;

        public RefreshTokenRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
        {
            var refreshTokenEntity = await context.RefreshTokens
                .AddAsync(refreshToken);

            return refreshTokenEntity.Entity;
        }

        public async Task RemoveToken(List<long> refreshTokensList)
        {
            await context.RefreshTokens
                .Where(x => refreshTokensList.Contains(x.Id))
                .ExecuteDeleteAsync();
        }
    }
}
