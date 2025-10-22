using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            // Récupérer toutes les réservations de l'utilisateur avec les détails
            Reservations = await _context.Reservations
                .Where(r => r.UserId == currentUser.Id)
                .Include(r => r.Boat)
                    .ThenInclude(b => b.Port) // Inclure le port du bateau
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // Handler pour annuler une réservation
        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            var reservation = await _context.Reservations
                .Include(r => r.Boat)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == currentUser.Id);

            if (reservation == null)
            {
                TempData["ErrorMessage"] = "Réservation introuvable.";
                return RedirectToPage();
            }

            // Vérifier si la réservation peut être annulée
            if (reservation.Status == ReservationStatus.Annulee || reservation.Status == ReservationStatus.Refusee)
            {
                TempData["ErrorMessage"] = "Cette réservation ne peut pas être annulée.";
                return RedirectToPage();
            }

            // Annuler la réservation
            reservation.Status = ReservationStatus.Annulee;
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Votre réservation a été annulée avec succès.";
            return RedirectToPage();
        }
    }
}