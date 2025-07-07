using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;


namespace SaillingLoc.Pages
{
    public class NotificationsModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public List<Notification> Notifications { get; set; }

        public NotificationsModel(UserManager<User> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                Notifications = await _dbContext.Notifications
                                    .Where(n => n.UserId == user.Id)
                                    .OrderByDescending(n => n.CreatedAt)
                                    .ToListAsync();

                foreach(var notif in Notifications.Where(n => !n.IsRead))
                {
                    notif.IsRead = true;
                }
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                Notifications = new List<Notification>();
            }
        }
    }
}
