using Microsoft.AspNetCore.Mvc.Filters;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.Profile;

namespace TestRating.API.Filters
{
    public class VerifyProfileFilter : IAsyncActionFilter
    {
        private readonly ITokenService<ProfileToken> tokenService;

        public VerifyProfileFilter(
            ITokenService<ProfileToken> tokenService)
        {
            this.tokenService = tokenService;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var token = context.HttpContext.Request.Cookies["token"];
            var profile = await tokenService.
                VerifyToken(token);

            context.HttpContext.Items.Add("profile", profile);

            await next();
        }
    }
}
