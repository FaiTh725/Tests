using MediatR;

namespace Authorization.Application.Commands.RefreshTokenEntity.AddRefreshToken
{
    public class AddRefreshTokenCommand : IRequest<long>
    {
        public string RefreshToken { get; set; } = string.Empty;

        public long UserId { get; set; }
    }
}
