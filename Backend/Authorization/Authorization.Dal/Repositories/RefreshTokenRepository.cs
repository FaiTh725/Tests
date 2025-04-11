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

        public async Task<RefreshToken?> GetRefreshToken(long id)
        {
            return await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<RefreshToken?> GetRefreshTokenWithUser(string token)
        {
            return await context.RefreshTokens
                .Where(x => x.Token == token)
                .Include(x => x.User)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveToken(string refreshToken)
        {
            await context.RefreshTokens
                .Where(x => x.Token == refreshToken)
                .ExecuteDeleteAsync();
        }

        public async Task UpdateRefreshToken(long id, RefreshToken refreshToken)
        {
            await context.RefreshTokens
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(y => y.Token, refreshToken.Token)
                    .SetProperty(y => y.ExpireOn, refreshToken.ExpireOn));
        }
    }
}
