using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models; // ✅ Ajout nécessaire pour accéder à ReservationStatus
using System.Linq;
using System.Threading.Tasks;

namespace SaillingLoc.Pages
{
    public class AdminDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int UserCount { get; set; }
        public int ReservationCount { get; set; }
        public int MessageCount { get; set; }
        public int BoatCount { get; set; }
        public int PendingReservationCount { get; set; }
        public int UnreadMessageCount { get; set; }

        public async Task OnGetAsync()
        {
            // ✅ Comptage total des entités
            UserCount = await _context.Users.CountAsync();
            BoatCount = await _context.Boats.CountAsync();

            // ✅ Réservations confirmées (status = "Acceptee")
            ReservationCount = await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Acceptee)
                .CountAsync();

            // ✅ Réservations en attente (status = "EnAttente")
            PendingReservationCount = await _context.Reservations
                .Where(r => r.Status == ReservationStatus.EnAttente)
                .CountAsync();

            // ✅ Messages non lus (IsRead = false)
            UnreadMessageCount = await _context.Messages
                .Where(m => !m.IsRead)
                .CountAsync();

            // ✅ Total des messages
            MessageCount = await _context.Messages.CountAsync();
        }
    }
}
