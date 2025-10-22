using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
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

        // Top Statistics Cards
        public int BoatCount { get; set; }
        public int ReservationCount { get; set; }
        public int ContractCount { get; set; }
        public decimal TotalRevenue { get; set; }

        // Reservation Status
        public int ConfirmedReservationCount { get; set; }
        public int PendingReservationCount { get; set; }

        // Financial Stats
        public int PendingPaymentCount { get; set; }

        // Bottom Stats
        public int UserCount { get; set; }
        public int BoatsInRentCount { get; set; }

        // Additional Stats
        public int UnreadMessageCount { get; set; }
        public int MessageCount { get; set; }

        public async Task OnGetAsync()
        {
            // ===== TOP STATISTICS CARDS =====
            
            // Total des bateaux actifs
            BoatCount = await _context.Boats.CountAsync();

            // Total des réservations (tous statuts confondus)
            var allReservations = await _context.Reservations.ToListAsync();
            ReservationCount = allReservations.Count;

            // Contrats actifs (à implémenter selon votre logique)
            ContractCount = 0; // TODO: Ajouter la logique des contrats si applicable

            // Revenus totaux (somme des prix de toutes les réservations confirmées)
            TotalRevenue = allReservations
                .Where(r => r.Status == ReservationStatus.Acceptee)
                .Sum(r => r.TotalPrice);

            // ===== RESERVATIONS PAR STATUT =====
            
            // Réservations confirmées
            ConfirmedReservationCount = allReservations
                .Count(r => r.Status == ReservationStatus.Acceptee);

            // Réservations en attente
            PendingReservationCount = allReservations
                .Count(r => r.Status == ReservationStatus.EnAttente);

            // ===== STATISTIQUES FINANCIÈRES =====
            
            // Paiements en attente (réservations confirmées non payées)
            // Vous pouvez ajouter une propriété IsPaid sur Reservation
            PendingPaymentCount = allReservations
                .Count(r => r.Status == ReservationStatus.EnAttente);

            // ===== STATISTIQUES DU BAS =====
            
            // Utilisateurs actifs
            UserCount = await _context.Users.CountAsync();

            // Bateaux actuellement en location
            // (réservations en cours aujourd'hui)
            var today = DateTime.UtcNow.Date;
            BoatsInRentCount = await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Acceptee 
                         && r.StartDate <= today 
                         && r.EndDate >= today)
                .Select(r => r.BoatId)
                .Distinct()
                .CountAsync();

            // ===== MESSAGES =====
            
            // Messages non lus
            UnreadMessageCount = await _context.Messages
                .Where(m => !m.IsRead)
                .CountAsync();

            // Total des messages
            MessageCount = await _context.Messages.CountAsync();
        }
    }
}