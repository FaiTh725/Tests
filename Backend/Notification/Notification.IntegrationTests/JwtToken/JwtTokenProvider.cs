using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Notification.IntegrationTests.JwtToken
{
    public static class JwtTokenProvider
    {
        public static string CreateJwt(JwtUserData jwtUser)
        {
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("w5pl9VHH4ZTjY1560OwGP7NwEOHtNeYWoyS35y30/bZMA8Zym8ppwp7qJMENf1QB")),
                SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Email, jwtUser.Email),
                new Claim(ClaimTypes.Role, jwtUser.Role),
                new Claim(ClaimTypes.Name, jwtUser.Name),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                audience: "Audience",
                issuer: "Issuer",
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddMinutes(15));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
