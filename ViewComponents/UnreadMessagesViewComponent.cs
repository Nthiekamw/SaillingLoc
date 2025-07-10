using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SaillingLoc.ViewComponents
{
    public class UnreadMessagesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UnreadMessagesViewComponent(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
                return View("~/ViewComponents/UnreadMessages/Default.cshtml", 0);

            var count = await _context.Messages
                .Where(m => m.ReceiverId == user.Id && !m.IsRead)
                .CountAsync();

            return View("~/ViewComponents/UnreadMessages/Default.cshtml", count);
        }
    }
}
