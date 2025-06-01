using CSharpFunctionalExtensions;
using Notification.Application.DTOs.Profiles;
using Notification.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Notification.Application.Implementations
{
    public class ProfileService : IProfileService
    {
        public Result<ProfileDTO> DecodeToken(string? token)
        {
            if(string.IsNullOrEmpty(token))
            {
                return Result.Failure<ProfileDTO>("Token is null or empty");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var email = jwtToken.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = jwtToken.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var role = jwtToken.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        
            if(email is null ||
                name is null ||
                role is null)
            {
                return Result.Failure<ProfileDTO>("Invalid token signature");
            }

            return Result.Success(new ProfileDTO
            {
                Email = email,
                Name = name,
                Role = role
            });
        }
    }
}
