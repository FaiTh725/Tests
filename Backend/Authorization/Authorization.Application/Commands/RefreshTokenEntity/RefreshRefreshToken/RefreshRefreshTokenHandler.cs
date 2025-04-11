using Application.Shared.Exceptions;
using Authorization.Domain.Interfaces;
using MediatR;

namespace Authorization.Application.Commands.RefreshTokenEntity.RefreshRefreshToken
{
    public class RefreshRefreshTokenHandler :
        IRequestHandler<RefreshRefreshTokenCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public RefreshRefreshTokenHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(RefreshRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await unitOfWork.RefreshTokenRepository
                .GetRefreshToken(request.Id);

            if (refreshToken is null)
            {
                throw new BadRequestException("Token doesnt exist");
            }

            refreshToken.Refresh(
                request.NewToken, request.NewExpireOn);

            await unitOfWork.RefreshTokenRepository
                .UpdateRefreshToken(request.Id, refreshToken);
        }
    }
}
