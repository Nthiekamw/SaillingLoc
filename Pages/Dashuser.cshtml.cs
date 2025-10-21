using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;


namespace SaillingLoc.Pages
{
    public class DashUserModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _um;

        public DashUserModel(ApplicationDbContext db, UserManager<User> um)
        {
            _db = db;
            _um = um;
        }
        
        public User CurrentUser { get; set; }


        public int BateauxActifs { get; set; }
        public int ReservationsConfirmees { get; set; }
        public int DemandesEnAttente { get; set; }
        public int MessagesNonLus { get; set; }
        public List<Reservation> Demandes { get; set; } = new();

        public async Task OnGetAsync()
        {
            var ownerId = _um.GetUserId(User);
            // Charger l'utilisateur connecté
CurrentUser = await _um.Users.FirstOrDefaultAsync(u => u.Id == ownerId);

            BateauxActifs = await _db.Boats
                .CountAsync(b => b.OwnerId == ownerId && b.IsActive);

            DemandesEnAttente = await _db.Reservations
                .CountAsync(r => r.Boat.OwnerId == ownerId && r.Status == ReservationStatus.EnAttente);

            ReservationsConfirmees = await _db.Reservations
                .CountAsync(r => r.Boat.OwnerId == ownerId && r.Status == ReservationStatus.Acceptee);

            MessagesNonLus = 0; // plug your message table here

            Demandes = await _db.Reservations
                .Where(r => r.Boat.OwnerId == ownerId && r.Status == ReservationStatus.EnAttente)
                .Include(r => r.Boat)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToPage("/Index");
        }
    }
}