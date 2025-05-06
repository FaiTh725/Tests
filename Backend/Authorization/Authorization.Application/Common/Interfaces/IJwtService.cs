using CSharpFunctionalExtensions;

namespace Authorization.Application.Common.Interfaces
{
    public interface IJwtService<TokenObj, TokenResponse>
    {
        string GenerateToken(TokenObj obj);

        Result<TokenResponse> DecodeToken(string token);

        string GenerateRefreshToken();
    }
}
