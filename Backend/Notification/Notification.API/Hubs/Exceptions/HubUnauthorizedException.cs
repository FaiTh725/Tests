using Microsoft.AspNetCore.SignalR;

namespace Notification.API.Hubs.Exceptions
{
    public class HubUnauthorizedException : HubException
    {
        public HubUnauthorizedException(string message) : 
            base(message)
        {}
    }
}
