using Authorization.Application.Commands.UserEntity.Login;
using Authorization.Application.Commands.UserEntity.Register;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
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

        private CookieOptions cookieOptions = new CookieOptions
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
            [FromQuery]LoginCommand request)
        {
            var userId = await mediator.Send(request);

            var user = await mediator.Send(new GetUserByIdQuery 
            { 
                Id = userId
            });

            var token = tokenService.GenerateToken(new UserTokenRequest
            {
                Email = user.Email,
                Role = user.Role,
                UserName = user.UserName,
            });

            Response.Cookies.Append("token", token, cookieOptions);

            return Ok(user);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register(
            RegisterCommand request)
        {
            var userId = await mediator.Send(request);

            var user = await mediator.Send(new GetUserByIdQuery
            {
                Id = userId
            });

            var token = tokenService.GenerateToken(new UserTokenRequest
            {
                Email = user.Email,
                Role = user.Role,
                UserName = user.UserName,
            });

            Response.Cookies.Append("token", token, cookieOptions);

            return Ok(user);
        }
    }
}
