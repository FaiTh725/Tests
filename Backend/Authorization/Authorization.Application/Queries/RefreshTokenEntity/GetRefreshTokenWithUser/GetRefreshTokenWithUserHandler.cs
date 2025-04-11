using Application.Shared.Exceptions;
using Authorization.Application.Contracts.RefreshToken;
using Authorization.Application.Contracts.User;
using Authorization.Domain.Interfaces;
using MediatR;

namespace Authorization.Application.Queries.RefreshTokenEntity.GetRefreshTokenWithUser
{
    public class GetRefreshTokenWithUserHandler :
        IRequestHandler<GetRefreshTokenWithUserQuery, UserRefreshTokenResponse>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetRefreshTokenWithUserHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<UserRefreshTokenResponse> Handle(GetRefreshTokenWithUserQuery request, CancellationToken cancellationToken)
        {
            var refreshToken = await unitOfWork.RefreshTokenRepository
                .GetRefreshTokenWithUser(request.Token, cancellationToken);

            if (refreshToken is null)
            {
                throw new NotFoundException("RefreshToken doesnt exist");
            }

            return new UserRefreshTokenResponse
            {
                Id = refreshToken.Id,
                ExpireOn = refreshToken.ExpireOn,
                Token = refreshToken.Token,
                User = new UserResponse
                {
                    Email = refreshToken.User.Email,
                    Role = refreshToken.User.RoleId,
                    UserName = refreshToken.User.UserName
                }
            };
        }
    }
}
