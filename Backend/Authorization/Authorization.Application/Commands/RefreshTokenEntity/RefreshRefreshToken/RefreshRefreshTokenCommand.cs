using MediatR;

namespace Authorization.Application.Commands.RefreshTokenEntity.RefreshRefreshToken
{
    public class RefreshRefreshTokenCommand : IRequest
    {
        public long Id { get; set; }

        public string NewToken { get; set; } = string.Empty;

        public DateTime NewExpireOn { get; set; }
    }
}
