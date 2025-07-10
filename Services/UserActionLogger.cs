using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Hubs;
using SaillingLoc.Models;

public class UserActionLogger : IUserActionLogger
{
    private readonly ApplicationDbContext _context;

    public UserActionLogger(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string action, string pageUrl, string userId, string ipAddress)
    {
        var log = new UserActionLog
        {
            Action = action,
            PageUrl = pageUrl,
            UserId = userId,
            IPAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };

        _context.UserActionLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
