using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SaillingLoc.Pages
{
    public class MyReservationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MyReservationsModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Reservation> Reservations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                Reservations = new();
                return;
            }

            Reservations = await _context.Reservations
                .Where(r => r.UserId == currentUser.Id)
                .Include(r => r.Boat) // Charge le bateau lié à chaque réservation
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
