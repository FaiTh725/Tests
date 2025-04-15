using Authorization.Application.Contracts.User;

namespace Authorization.Application.Contracts.RefreshToken
{
    public class UserRefreshTokenResponse
    {
        public long Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpireOn { get; set; }

        public required UserResponse User { get; set; }
    }
}
