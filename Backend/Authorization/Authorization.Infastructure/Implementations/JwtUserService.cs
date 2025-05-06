using Application.Shared.Exceptions;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Infrastructure.Configurations;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authorization.Infrastructure.Implementations
{
    public class JwtUserService :
        IJwtService<UserTokenRequest, UserTokenResponse>
    {
        private readonly IConfiguration configuration;

        public JwtUserService(
            IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Result<UserTokenResponse> DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
        
            var email = jwtSecurityToken.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var role = jwtSecurityToken.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            var userName = jwtSecurityToken.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            if(email is null ||
                role is null ||
                userName is null)
            {
                return Result.Failure<UserTokenResponse>("Token has invalid signature");
            }

            return Result.Success(new UserTokenResponse
            {
                Email = email,
                Role = role,
                UserName = userName
            });
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        public string GenerateToken(UserTokenRequest user)
        {
            var jwtTokenConf = configuration
                .GetSection("JwtSettings")
                .Get<JwtTokenConf>() ??
                throw new AppConfigurationException("Jwt token configuration");

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConf.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                audience: jwtTokenConf.Audience,
                issuer: jwtTokenConf.Issuer,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddMinutes(jwtTokenConf.ExpirationTime
                ));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
