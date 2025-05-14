namespace Authorization.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<(string, string)> Refresh(string? oldRefreshToken, CancellationToken cancellationToken = default);

        Task Logout(string? refreshToken, CancellationToken cancellationToken = default);
    }
}
