using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaillingLoc.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public UsersModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<User> Users { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty]
        public User EditedUser { get; set; }

        [BindProperty]
        public string UserIdToDelete { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(u =>
                    u.FirstName.Contains(SearchTerm) ||
                    u.LastName.Contains(SearchTerm) ||
                    u.Email.Contains(SearchTerm));
            }

            Users = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var user = await _context.Users.FindAsync(EditedUser.Id);
            if (user == null)
                return NotFound();

            user.FirstName = EditedUser.FirstName;
            user.LastName = EditedUser.LastName;
            user.Email = EditedUser.Email;

            await _context.SaveChangesAsync();
            return RedirectToPage(new { searchTerm = SearchTerm });
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var user = await _context.Users.FindAsync(UserIdToDelete);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToPage(new { searchTerm = SearchTerm });
        }
    }
}
