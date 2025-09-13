














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

            // Vérifie si l'utilisateur est admin
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            IQueryable<Message> messagesQuery = _context.Messages.Where(m => !m.IsRead);

            if (!isAdmin)
            {
                // Les autres utilisateurs ne comptent que leurs messages
                messagesQuery = messagesQuery.Where(m => m.ReceiverId == user.Id);
            }

            var count = await messagesQuery.CountAsync();

            return View("~/ViewComponents/UnreadMessages/Default.cshtml", count);
        }
    }
}


































// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using SaillingLoc.Data;
// using SaillingLoc.Models;
// using System.Linq;
// using System.Threading.Tasks;

// namespace SaillingLoc.ViewComponents
// {
//     public class UnreadMessagesViewComponent : ViewComponent
//     {
//         private readonly ApplicationDbContext _context;
//         private readonly UserManager<User> _userManager;

//         public UnreadMessagesViewComponent(ApplicationDbContext context, UserManager<User> userManager)
//         {
//             _context = context;
//             _userManager = userManager;
//         }

//         public async Task<IViewComponentResult> InvokeAsync()
//         {
//             var user = await _userManager.GetUserAsync(HttpContext.User);

//             if (user == null)
//                 return View("~/ViewComponents/UnreadMessages/Default.cshtml", 0);

//             var count = await _context.Messages
//                 .Where(m => m.ReceiverId == user.Id && !m.IsRead)
//                 .CountAsync();

//             return View("~/ViewComponents/UnreadMessages/Default.cshtml", count);
//         }
//     }
// }
