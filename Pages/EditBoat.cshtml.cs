using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using SaillingLoc.Models;
using SaillingLoc.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SaillingLoc.Pages
{
    public class EditBoatModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<EditBoatModel> _logger;

        public EditBoatModel(ApplicationDbContext context, UserManager<User> userManager, ILogger<EditBoatModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public Boat Boat { get; set; }

        [BindProperty]
        public IFormFile? Photo { get; set; }

        public SelectList Ports { get; set; }
        public SelectList BoatTypes { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Boat = await _context.Boats.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            if (Boat == null)
                return NotFound();

            // Récupérer user connecté et s'assurer que les champs UserId/OwnerId existent
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                // si UserId manquant, assigner
                if (string.IsNullOrEmpty(Boat.UserId))
                    Boat.UserId = currentUser.Id;

                if (string.IsNullOrEmpty(Boat.OwnerId))
                    Boat.OwnerId = currentUser.Id;
            }

            await LoadSelectListsAsync(Boat.BoatTypeId, Boat.PortId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Assignation côté serveur du UserId si connecté (évite la validation "UserId required")
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                if (string.IsNullOrEmpty(Boat.UserId))
                    Boat.UserId = currentUser.Id;
                if (string.IsNullOrEmpty(Boat.OwnerId))
                    Boat.OwnerId = currentUser.Id;
            }

            // Si la validation échoue, on recharge les listes et on log les erreurs
            if (!ModelState.IsValid)
            {
                // log ModelState errors pour debug
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        _logger.LogWarning("ModelState error: Field '{Field}' -> '{Error}'", kv.Key, error.ErrorMessage);
                    }
                }

                await LoadSelectListsAsync(Boat.BoatTypeId, Boat.PortId);
                return Page();
            }

            var boatToUpdate = await _context.Boats.FindAsync(Boat.Id);
            if (boatToUpdate == null)
                return NotFound();

            // Optionnel : contrôle d'autorisation (décommenter si tu veux restreindre)
            // var cur = await _userManager.GetUserAsync(User);
            // if (cur != null && boatToUpdate.OwnerId != cur.Id && !User.IsInRole("Admin")) return Forbid();

            // Mise à jour des champs
            boatToUpdate.Name = Boat.Name;
            boatToUpdate.DailyPrice = Boat.DailyPrice;
            boatToUpdate.PricePerDay = Boat.PricePerDay; // si tu utilises ce champ
            boatToUpdate.PortId = Boat.PortId;
            boatToUpdate.BoatTypeId = Boat.BoatTypeId;
            boatToUpdate.Description = Boat.Description;
            boatToUpdate.Brand = Boat.Brand;
            boatToUpdate.Model = Boat.Model;
            boatToUpdate.Length = Boat.Length;
            boatToUpdate.EngineType = Boat.EngineType;
            boatToUpdate.MaxPassengers = Boat.MaxPassengers;
            boatToUpdate.SkipperRequired = Boat.SkipperRequired;
            boatToUpdate.Latitude = Boat.Latitude;
            boatToUpdate.Longitude = Boat.Longitude;
            boatToUpdate.Capacity = Boat.Capacity;
            boatToUpdate.OwnerId = Boat.OwnerId;
            boatToUpdate.UserId = Boat.UserId; // *** important : on set UserId aussi ***
            boatToUpdate.UpdatedAt = DateTime.UtcNow;

            // Gestion photo
            if (Photo != null && Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "boats");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(Photo.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var fs = new FileStream(filePath, FileMode.Create);
                await Photo.CopyToAsync(fs);

                boatToUpdate.Photo = $"/uploads/boats/{fileName}";
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }

        private async Task LoadSelectListsAsync(int? selectedBoatTypeId = null, int? selectedPortId = null)
        {
            var ports = await _context.Ports.AsNoTracking().ToListAsync();
            var boatTypes = await _context.BoatTypes.AsNoTracking().ToListAsync();

            Ports = new SelectList(ports, "Id", "Name", selectedPortId);
            BoatTypes = new SelectList(boatTypes, "Id", "Name", selectedBoatTypeId);
        }
    }
}
