using Application.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Notification.Application.Interfaces;

namespace Notification.API.Filters
{
    public class DecodeTokenFilter : IActionFilter
    {
        private readonly IProfileService profileService;

        public DecodeTokenFilter(
            IProfileService profileService)
        {
            this.profileService = profileService;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {}

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Cookies["token"];
            var decodedTokenResult = profileService.DecodeToken(token);

            if (decodedTokenResult.IsFailure)
            {
                throw new UnauthorizeException("Auth token has invalid signature");
            }

            context.HttpContext.Items.Add("profile", decodedTokenResult.Value);
        }
    }
}
