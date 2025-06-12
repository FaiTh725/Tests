using MediatR;

namespace Authorization.Application.Commands.RefreshTokenEntity.DeleteRefreshToken
{
    public class DeleteRefreshTokenCommand : IRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
