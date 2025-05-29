using Application.Shared.Exceptions;
using Authorization.Application.Commands.RefreshTokenEntity.DeleteRefreshToken;
using Authorization.Application.Commands.RefreshTokenEntity.RefreshRefreshToken;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Domain.Interfaces;
using MediatR;

namespace Authorization.Application.Common.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IJwtService<UserTokenRequest, UserTokenResponse> tokenService;
        private readonly IMediator mediator;

        public AuthService(
            IUnitOfWork unitOfWork, 
            IJwtService<UserTokenRequest, UserTokenResponse> tokenService,
            IMediator mediator)
        {
            this.unitOfWork = unitOfWork;
            this.tokenService = tokenService;
            this.mediator = mediator;
        }

        public async Task Logout(
            string? refreshToken, 
            CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await mediator.Send(new DeleteRefreshTokenCommand
                {
                    RefreshToken = refreshToken
                },
                cancellationToken);
            }
        }

        public async Task<(string, string)> Refresh(
            string? oldRefreshToken, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(oldRefreshToken))
            {
                throw new UnauthorizeException("Refresh token is missing");
            }

            var existedRefreshToken = await unitOfWork.RefreshTokenRepository
                .GetRefreshTokenWithUser(oldRefreshToken, cancellationToken);

            if (existedRefreshToken is null ||
                existedRefreshToken.ExpireOn < DateTime.UtcNow)
            {
                throw new UnauthorizeException("Refresh token is missing or expired");
            }

            var accessToken = tokenService.GenerateToken(new UserTokenRequest
            {
                Email = existedRefreshToken.User.Email,
                Role = existedRefreshToken.User.RoleId,
                UserName = existedRefreshToken.User.UserName
            });
            var newRefreshToken = tokenService.GenerateRefreshToken();

            await mediator.Send(new RefreshRefreshTokenCommand
            {
                Id = existedRefreshToken.Id,
                NewToken = accessToken,
            }, cancellationToken);

            return (accessToken, newRefreshToken);
        }
    }
}
