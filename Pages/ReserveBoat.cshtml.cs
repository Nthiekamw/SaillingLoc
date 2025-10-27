using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;

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

        // Clé secrète reCAPTCHA fournie (garde-la confidentielle)
        private const string RecaptchaSecretKey = "6LdEqvgrAAAAAE_H7qrpZgCYdbxcHh0TI_-nkVzR";

        public async Task<IActionResult> OnGetAsync()
        {
            Boat = await _context.Boats.FindAsync(Id);
            if (Boat == null)
            {
                return NotFound();
            }

            // Initialisation par défaut des dates côté serveur si non fournies
            if (StartDate == default) StartDate = DateTime.Today;
            if (EndDate == default) EndDate = DateTime.Today.AddDays(1);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Recharger le bateau d'abord (utile en cas d'erreur du captcha)
            Boat = await _context.Boats.FindAsync(Id);
            if (Boat == null)
            {
                return NotFound();
            }

            // 1) Vérification du captcha
            var captchaResponse = Request.Form["g-recaptcha-response"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(captchaResponse))
            {
                ModelState.AddModelError(string.Empty, "Veuillez valider le captcha.");
                return Page();
            }

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // POST vers l'API de verification (on peut aussi utiliser Get, mais POST est recommandé)
                var verifyUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={Uri.EscapeDataString(RecaptchaSecretKey)}&response={Uri.EscapeDataString(captchaResponse)}";
                var httpResponse = await client.PostAsync(verifyUrl, null);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Erreur lors de la vérification du captcha.");
                    return Page();
                }

                var content = await httpResponse.Content.ReadAsStringAsync();
                var captchaResult = JsonSerializer.Deserialize<RecaptchaResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (captchaResult == null || !captchaResult.success)
                {
                    ModelState.AddModelError(string.Empty, "Échec de la vérification du captcha. Veuillez réessayer.");
                    return Page();
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Impossible de vérifier le captcha. Veuillez réessayer plus tard.");
                return Page();
            }

            // 2) Vérification de l'utilisateur connecté
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Vous devez être connecté pour effectuer une réservation.");
                return Page();
            }

            // 3) Validation des dates côté serveur
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Veuillez vérifier les données saisies.");
                return Page();
            }

            if (StartDate >= EndDate)
            {
                ModelState.AddModelError(string.Empty, "La date de fin doit être supérieure à la date de début.");
                return Page();
            }

            // Date de début dans le futur (compare sur Date pour éviter problème fuseau)
            if (StartDate.Date < DateTime.UtcNow.Date)
            {
                ModelState.AddModelError(string.Empty, "La date de début doit être dans le futur.");
                return Page();
            }

            // Vérifier chevauchement de réservations existantes
            var isOverlap = await _context.Reservations.AnyAsync(r =>
                r.BoatId == Id &&
                ((StartDate >= r.StartDate && StartDate < r.EndDate) ||
                 (EndDate > r.StartDate && EndDate <= r.EndDate) ||
                 (StartDate <= r.StartDate && EndDate >= r.EndDate))
            );

            if (isOverlap)
            {
                ModelState.AddModelError(string.Empty, "Ce bateau est déjà réservé à ces dates.");
                return Page();
            }

            // Calcul du prix et création de la réservation
            double totalDays = (EndDate - StartDate).TotalDays;
            if (totalDays <= 0)
            {
                ModelState.AddModelError(string.Empty, "La durée de réservation doit être d'au moins 1 jour.");
                return Page();
            }

            decimal totalPrice = (decimal)totalDays * Boat.PricePerDay;

            var reservation = new Reservation
            {
                Reference = $"RES-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                BoatId = Id,
                UserId = user.Id,
                BoatOwnerId = Boat.UserId,
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

    // Classe pour parser la réponse du captcha
    public class RecaptchaResponse
    {
        public bool success { get; set; }
        public float score { get; set; } // uniquement pour v3
        public string action { get; set; } // uniquement pour v3
        public string challenge_ts { get; set; }
        public string hostname { get; set; }
        // public string[] error-codes { get; set; } // possible erreurs renvoyées par l'API
    }
}
