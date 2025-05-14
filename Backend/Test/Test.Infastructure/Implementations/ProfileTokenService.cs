using CSharpFunctionalExtensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileEntity;

namespace Test.Infrastructure.Implementations
{
    public class ProfileTokenService : ITokenService<ProfileToken>
    {
        public Result<ProfileToken> DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var email = jwtToken.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = jwtToken.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var role = jwtToken.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

            if (email is null ||
                name is null ||
                role is null)
            {
                return Result.Failure<ProfileToken>("Invalid token signature");
            }

            return Result.Success(new ProfileToken 
            { 
                Email = email,
                Name = name
            });
        }
    }
}
