using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SaillingLoc.Pages
{
    public class ReserveBoatModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReserveBoatModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } // ID du bateau

        [BindProperty]
        public DateTime StartDate { get; set; }

        [BindProperty]
        public DateTime EndDate { get; set; }

        public Boat Boat { get; set; }

        public Reservation Reservation { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Boat = await _context.Boats.FindAsync(Id);
            if (Boat == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid || StartDate >= EndDate || user == null)
            {
                ModelState.AddModelError(string.Empty, "Veuillez vérifier les dates de réservation ou assurez-vous d'être connecté.");
                Boat = await _context.Boats.FindAsync(Id);
                return Page();
            }

            if (StartDate < DateTime.UtcNow)
            {
                ModelState.AddModelError(string.Empty, "La date de début doit être dans le futur.");
                Boat = await _context.Boats.FindAsync(Id);
                return Page();
            }

            var boat = await _context.Boats.FindAsync(Id);
            if (boat == null)
            {
                return NotFound();
            }

            var isOverlap = await _context.Reservations.AnyAsync(r =>
                r.BoatId == Id &&
                ((StartDate >= r.StartDate && StartDate < r.EndDate) ||
                 (EndDate > r.StartDate && EndDate <= r.EndDate) ||
                 (StartDate <= r.StartDate && EndDate >= r.EndDate))
            );

            if (isOverlap)
            {
                ModelState.AddModelError(string.Empty, "Ce bateau est déjà réservé à ces dates.");
                Boat = boat;
                return Page();
            }

            double totalDays = (EndDate - StartDate).TotalDays;
            decimal totalPrice = (decimal)totalDays * boat.PricePerDay;

            var boatOwnerId = boat.UserId; // ✅ Récupération du propriétaire

            var reservation = new Reservation
            {
                Reference = $"RES-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                BoatId = Id,
                UserId = user.Id,
                BoatOwnerId = boatOwnerId, // ✅ Ajout ici
                StartDate = StartDate,
                EndDate = EndDate,
                TotalPrice = totalPrice,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            Reservation = reservation;
            TempData["SuccessMessage"] = "Votre réservation a bien été enregistrée.";

            return RedirectToPage("/MyReservations");
        }
    }
}
