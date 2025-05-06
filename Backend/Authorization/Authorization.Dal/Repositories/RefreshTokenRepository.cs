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

        public async Task<RefreshToken> AddRefreshToken(
            RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            var refreshTokenEntity = await context.RefreshTokens
                .AddAsync(refreshToken, cancellationToken);

            return refreshTokenEntity.Entity;
        }

        public async Task<RefreshToken?> GetRefreshToken(
            long id, CancellationToken cancellationToken = default)
        {
            return await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<RefreshToken?> GetRefreshTokenByUserId(
            long userId, CancellationToken cancellationToken = default)
        {
            return await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.UserId == userId, 
                cancellationToken);
        }

        public async Task<RefreshToken?> GetRefreshTokenWithUser(
            string token, CancellationToken cancellationToken = default)
        {
            return await context.RefreshTokens
                .Where(x => x.Token == token)
                .Include(x => x.User)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task RemoveToken(
            string refreshToken, CancellationToken cancellationToken = default)
        {
            await context.RefreshTokens
                .Where(x => x.Token == refreshToken)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task UpdateRefreshToken(
            long id, RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            await context.RefreshTokens
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(y => y.Token, refreshToken.Token)
                    .SetProperty(y => y.ExpireOn, refreshToken.ExpireOn), 
                cancellationToken);
        }
    }
}
