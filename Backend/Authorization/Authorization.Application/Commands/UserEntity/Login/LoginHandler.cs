using Application.Shared.Exceptions;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Domain.Entities;
using Authorization.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Authorization.Application.Commands.UserEntity.Login
{
    public class LoginHandler :
        IRequestHandler<LoginCommand, (long, string)>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHashService hashService;
        private readonly IJwtService<UserTokenRequest, UserTokenResponse> tokenService;
        private readonly IConfiguration configuration;

        public LoginHandler(
            IUnitOfWork unitOfWork,
            IHashService hashService,
            IJwtService<UserTokenRequest, UserTokenResponse> tokenService,
            IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.hashService = hashService;
            this.tokenService = tokenService;
            this.configuration = configuration;
        }

        public async Task<(long, string)> Handle(
            LoginCommand request, 
            CancellationToken cancellationToken)
        {
            var existedUser = await unitOfWork.UserRepository
                .GetUserByEmail(request.Email, cancellationToken);

            if (existedUser is null)
            {
                throw new BadRequestException($"Email {request.Email} isnt registered");
            }

            if(!hashService.VerifyHash(
                request.Password,
                existedUser.PasswordHash))
            {
                throw new BadRequestException($"Email invalid password or email");
            }

            var newRefreshToken = tokenService.GenerateRefreshToken();

            var userOldRefreshToken = await unitOfWork.RefreshTokenRepository
                .GetRefreshTokenByUserId(existedUser.Id, cancellationToken);

            var tokenLifeTime = configuration
                .GetValue<int>("JwtSettings:ExpirationTimeRefreshTokenInDays");

            if (userOldRefreshToken is null)
            {
                var newUserRefreshToken = RefreshToken.Initialize(
                    newRefreshToken, existedUser, DateTime.UtcNow.AddDays(tokenLifeTime));

                if(newUserRefreshToken.IsFailure)
                {
                    throw new InternalServerErrorException("Error with refresh token");
                }

                await unitOfWork.RefreshTokenRepository
                    .AddRefreshToken(newUserRefreshToken.Value, cancellationToken);
            }
            else
            {
                userOldRefreshToken.Refresh(
                    newRefreshToken, DateTime.UtcNow.AddDays(tokenLifeTime));

                await unitOfWork.RefreshTokenRepository
                    .UpdateRefreshToken(userOldRefreshToken.Id, userOldRefreshToken, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return (existedUser.Id, newRefreshToken);
        }
    }
}
