using Application.Shared.Exceptions;
using Authorization.Application.Commands.RefreshTokenEntity.DeleteRefreshToken;
using Authorization.Application.Commands.RefreshTokenEntity.RefreshRefreshToken;
using Authorization.Application.Commands.UserEntity.Login;
using Authorization.Application.Commands.UserEntity.Register;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Application.Queries.RefreshTokenEntity.GetRefreshTokenWithUser;
using Authorization.Application.Queries.UserEntity.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IJwtService<UserTokenRequest, UserTokenResponse> tokenService;

        private readonly CookieOptions cookieOptions = new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            HttpOnly = true
        };

        public AuthController(
            IMediator mediator,
            IJwtService<UserTokenRequest, UserTokenResponse> tokenService)
        {
            this.mediator = mediator;
            this.tokenService = tokenService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Login(
            [FromQuery]LoginCommand request,
            CancellationToken cancellationToken)
        {
            var userEntered = await mediator.Send(request, cancellationToken);

            var user = await mediator.Send(new GetUserByIdQuery 
            { 
                Id = userEntered.Item1
            }, 
            cancellationToken);

            var accessToken = tokenService.GenerateToken(new UserTokenRequest
            {
                Email = user.Email,
                Role = user.Role,
                UserName = user.UserName,
            });

            Response.Cookies.Append("token", accessToken, cookieOptions);
            Response.Cookies.Append("refresh_token", userEntered.Item2, cookieOptions);

            return Ok(user);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register(
            RegisterCommand request,
            CancellationToken cancellationToken)
        {
            var userRegistered = await mediator.Send(request, cancellationToken);

            var user = await mediator.Send(new GetUserByIdQuery
            {
                Id = userRegistered.Item1
            }, 
            cancellationToken);

            var token = tokenService.GenerateToken(new UserTokenRequest
            {
                Email = user.Email,
                Role = user.Role,
                UserName = user.UserName,
            });

            Response.Cookies.Append("token", token, cookieOptions);
            Response.Cookies.Append("refresh_token", userRegistered.Item2, cookieOptions);

            return Ok(user);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Refresh(
            CancellationToken cancellationToken)
        {
            var oldRefreshToken = Request.Cookies["refresh_token"];
            if(string.IsNullOrWhiteSpace(oldRefreshToken))
            {
                throw new UnauthorizeException("Refresh token is missing");
            }

            var existedRefreshToken = await mediator.Send(
                new GetRefreshTokenWithUserQuery
                {
                    Token = oldRefreshToken
                },
                cancellationToken);
            if(existedRefreshToken.ExpireOn < DateTime.UtcNow)
            {
                throw new UnauthorizeException("Refresh token is expired");
            }

            var accessToken = tokenService.GenerateToken(new UserTokenRequest
            {
                Email = existedRefreshToken.User.Email,
                Role = existedRefreshToken.User.Role,
                UserName = existedRefreshToken.User.UserName
            });
            var newRefreshToken = tokenService.GenerateRefreshToken();

            await mediator.Send(new RefreshRefreshTokenCommand
            {
                Id = existedRefreshToken.Id,
                NewToken = newRefreshToken
            },
            cancellationToken);

            Response.Cookies.Append("token", accessToken, cookieOptions);
            Response.Cookies.Append("refresh_token", newRefreshToken, cookieOptions);

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Logout(
            CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if(!string.IsNullOrWhiteSpace(refreshToken))
            {
                await mediator.Send(new DeleteRefreshTokenCommand
                {
                    RefreshToken = refreshToken
                }, 
                cancellationToken);

                Response.Cookies.Delete("refresh_token");
                Response.Cookies.Delete("token");
            }

            return NoContent();
        }
    }
}
