using Application.Shared.Exceptions;
using Authorization.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Authorization.Application.Commands.RefreshTokenEntity.RefreshRefreshToken
{
    public class RefreshRefreshTokenHandler :
        IRequestHandler<RefreshRefreshTokenCommand>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;

        public RefreshRefreshTokenHandler(
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
        }

        public async Task Handle(
            RefreshRefreshTokenCommand request, 
            CancellationToken cancellationToken)
        {
            var refreshToken = await unitOfWork.RefreshTokenRepository
                .GetRefreshToken(request.Id, cancellationToken);

            if (refreshToken is null)
            {
                throw new BadRequestException("Token doesnt exist");
            }

            var tokenLifeTime = configuration
                .GetValue<int>("JwtSettings:ExpirationTimeRefreshTokenInDays");

            refreshToken.Refresh(
                request.NewToken, DateTime.UtcNow.AddDays(tokenLifeTime));

            await unitOfWork.RefreshTokenRepository
                .UpdateRefreshToken(request.Id, refreshToken, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
