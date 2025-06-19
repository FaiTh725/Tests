using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Test.API.Hubs
{
    public class EmailBaseUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
