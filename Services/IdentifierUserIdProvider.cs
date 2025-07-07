using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SaillingLoc.Services
{
    public class IdentifierUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
