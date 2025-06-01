using Microsoft.AspNetCore.SignalR;

namespace Notification.API.Hubs.Exceptions
{
    public class HubMissingTokenException : HubException
    {
        public HubMissingTokenException(string message) : 
            base(message)
        {}
    }
}
