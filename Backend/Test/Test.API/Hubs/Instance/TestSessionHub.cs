using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Test.API.Hubs.Interfaces;
using Test.Application.Common.Interfaces;

namespace Test.API.Hubs.Instance
{
    [Authorize]
    public class TestSessionHub : Hub<ITestSessionHub>
    {
        private readonly ICacheService cacheService;

        public TestSessionHub(
            ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public override async Task OnConnectedAsync()
        {
            var userEmail = Context.UserIdentifier;
            
            if(userEmail is null)
            {
                return;
            }

            await cacheService.SetData(userEmail, Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userEmail = Context.UserIdentifier;

            if (userEmail is null)
            {
                return;
            }

            await cacheService.RemoveData(userEmail);
        }
    }
}
