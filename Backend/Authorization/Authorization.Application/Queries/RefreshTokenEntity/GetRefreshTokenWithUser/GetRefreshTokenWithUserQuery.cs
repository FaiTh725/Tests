using Authorization.Application.Contracts.RefreshToken;
using MediatR;

namespace Authorization.Application.Queries.RefreshTokenEntity.GetRefreshTokenWithUser
{
    public class GetRefreshTokenWithUserQuery : IRequest<UserRefreshTokenResponse>
    {
        public string Token { get; set; } = string.Empty;
    }
}
