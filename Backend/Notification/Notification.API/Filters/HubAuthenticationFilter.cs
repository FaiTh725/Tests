using Microsoft.AspNetCore.SignalR;
using Notification.API.Hubs.Exceptions;
using System.Security.Claims;

namespace Notification.API.Filters
{
    public class HubAuthenticationFilter : IHubFilter
    {
        public async ValueTask<object> InvokeMethodAsync(
            HubInvocationContext context, 
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            VerifyUser(context.Context);

            return await next(context);
        }

        public Task OnConnectedAsync(
            HubLifetimeContext context, 
            Func<HubLifetimeContext, Task> next)
        {
            VerifyUser(context.Context);

            return next(context);
        }

        private void VerifyUser(HubCallerContext context)
        {
            if (context.User is null)
            {
                throw new HubException("User is missing");
            }

            if (!context.User!.Identity!.IsAuthenticated)
            {
                throw new HubUnauthorizedException("User isnt authorized");
            }

            var userEmail = context.UserIdentifier;

            if (userEmail is null)
            {
                throw new HubMissingTokenException("Token is missing");
            }

            context.Items.Add("email", userEmail);
        }
    }
}
