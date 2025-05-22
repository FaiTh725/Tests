using Authorization.Application.Commands.Email.SendConfirmCode;
using Authorization.Application.Commands.Email.VerifyCode;
using Authorization.Application.Commands.UserEntity.Login;
using Authorization.Application.Commands.UserEntity.Register;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Application.Queries.UserEntity.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Authorization.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IJwtService<UserTokenRequest, UserTokenResponse> tokenService;
        private readonly IAuthService authService;

        private readonly CookieOptions cookieOptions = new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            HttpOnly = true
        };

        public AuthController(
            IMediator mediator,
            IJwtService<UserTokenRequest, UserTokenResponse> tokenService,
            IAuthService authService)
        {
            this.mediator = mediator;
            this.tokenService = tokenService;
            this.authService = authService;
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

            var (accessToken, newRefreshToken) = await authService
                .Refresh(oldRefreshToken, cancellationToken);

            Response.Cookies.Append("token", accessToken, cookieOptions);
            Response.Cookies.Append("refresh_token", newRefreshToken, cookieOptions);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Logout(
            CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refresh_token"];

            await authService.Logout(refreshToken, cancellationToken);
            
            Response.Cookies.Delete("refresh_token");
            Response.Cookies.Delete("token");

            return NoContent();
        }

        [HttpPost("[action]")]
        [EnableRateLimiting("confirm_email")]
        public async Task<IActionResult> SendEmailConfirmationCode(
            SendConfirmCodeCommand request, CancellationToken cancellationToken)
        {
            await mediator.Send(request, cancellationToken);

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> VerifyConfirmationCode(
            VerifyCodeCommand request, CancellationToken cancellationToken)
        {
            await mediator.Send(request, cancellationToken);

            return Ok();
        }
    }
}
