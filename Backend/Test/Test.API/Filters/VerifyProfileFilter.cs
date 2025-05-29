using Microsoft.AspNetCore.Mvc.Filters;
using Test.Application.Common.Interfaces;

namespace Test.API.Filters
{
    public class VerifyProfileFilter :
        Attribute, IAsyncActionFilter
    {
        private readonly IProfileService profileService;

        public VerifyProfileFilter(
            IProfileService profileService)
        {
            this.profileService = profileService;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, 
            ActionExecutionDelegate next)
        {
            var token = context.HttpContext.Request.Cookies["token"];
            var profile = await profileService
                .DecodeProfileFromToken(token);

            context.ActionArguments.Add("profile", profile);
        }
    }
}
