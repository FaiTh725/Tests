using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Test.Integration.Tests.JwtAuthenticationMock.cs
{
    public class JwtAuthHandler : JwtBearerHandler
    {
        public JwtAuthHandler(
            IOptionsMonitor<JwtBearerOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) :
            base(options, logger, encoder, clock)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var isTokenProvided = Context.Request.Headers
                .TryGetValue("Authorization", out var authHeaderValue);

            if (!isTokenProvided)
            {
                return Task.FromResult(AuthenticateResult.Fail("Not token provided"));
            }

            var jwtToken = authHeaderValue.FirstOrDefault()?
                .Split(" ")
                .LastOrDefault();

            if (string.IsNullOrEmpty(jwtToken))
            {
                return Task.FromResult(AuthenticateResult.Fail("Not token provided"));
            }

            var token = new JwtSecurityTokenHandler().ReadToken(jwtToken) as JwtSecurityToken;

            if (token is not null)
            {
                var claims = token.Claims;
                var identity = new ClaimsIdentity(claims, "Bearer",
                    ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                var authTiket = new AuthenticationTicket(new ClaimsPrincipal(identity), "Bearer");

                return Task.FromResult(AuthenticateResult.Success(authTiket));
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("Not token provided"));
            }
        }
    }
}
