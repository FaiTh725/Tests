using Microsoft.AspNetCore.SignalR;
using Test.API.Hubs.Instance;
using Test.API.Hubs.Interfaces;
using Test.Application.Common.Interfaces;

namespace Test.API.Services
{
    public class SignalRTestNotificationService : ITestNotificationService
    {
        private readonly IHubContext<TestSessionHub, ITestSessionHub> sessionHub;
        private readonly ICacheService cacheService;
        private readonly ILogger<SignalRTestNotificationService> logger;

        public SignalRTestNotificationService(
            IHubContext<TestSessionHub, ITestSessionHub> sessionHub, 
            ICacheService cacheService,
            ILogger<SignalRTestNotificationService> logger)
        {
            this.sessionHub = sessionHub;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task NotifyTestOver(string userEmail, long sessionId)
        {
            var userConnectionIdResult = await cacheService
                .GetData<string>(userEmail);
        
            if(userConnectionIdResult.IsFailure)
            {
                logger.LogInformation("User connection id isnt in cache or already disconnected");
                return;
            }

            await sessionHub.Clients
                .Client(userConnectionIdResult.Value)
                .TestStoped(sessionId);
        }
    }
}
