using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class LogsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public List<UserActionLog> Logs { get; set; }

    public LogsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        Logs = await _context.UserActionLogs
            .Include(l => l.User)
            .OrderByDescending(l => l.Timestamp)
            .Take(100)
            .ToListAsync();
    }
}
