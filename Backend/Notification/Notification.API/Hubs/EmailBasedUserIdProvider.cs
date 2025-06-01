using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Notification.API.Hubs
{
    public class EmailBasedUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
