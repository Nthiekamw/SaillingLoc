using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaillingLoc.Pages
{
    public class ManageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ManageModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Reservation> Reservations { get; set; }
        public string CurrentUserId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge(); // Redirige vers login si non connecté
            }

            CurrentUserId = currentUser.Id;

            Reservations = await _context.Reservations
                .Where(r => r.BoatOwnerId == CurrentUserId)
                .Include(r => r.Boat)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAccepterAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null || reservation.BoatOwnerId != currentUser.Id)
                return Unauthorized();

            reservation.Status = ReservationStatus.Acceptee;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRefuserAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null || reservation.BoatOwnerId != currentUser.Id)
                return Unauthorized();

            reservation.Status = ReservationStatus.Refusee;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
