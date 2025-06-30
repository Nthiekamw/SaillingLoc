using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SaillingLoc.Pages.Proprietaire
{
    [Authorize(Roles = "Admin,Proprietaire")]
    public class AjouterBateauModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AjouterBateauModel(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public Boat Boat { get; set; }

        [BindProperty]
        public IFormFile Photo { get; set; }

        public List<SelectListItem> BoatTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Ports { get; set; } = new List<SelectListItem>();

        public void OnGet()
        {
            LoadBoatTypes();
            LoadPorts();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Récupérer UserId
            Boat.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(Boat.UserId))
            {
                ModelState.AddModelError(string.Empty, "Utilisateur non identifié.");
            }

            if (Photo == null || Photo.Length == 0)
            {
                ModelState.AddModelError("Photo", "Une photo est obligatoire.");
            }

            if (!ModelState.IsValid)
            {
                LoadBoatTypes();
                LoadPorts();
                return Page();
            }

            Boat.CreatedAt = DateTime.UtcNow;
            Boat.UpdatedAt = DateTime.UtcNow;

            // Sauvegarde photo
            var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(Photo.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Photo.CopyToAsync(stream);
            }

            Boat.Photo = $"/images/{uniqueFileName}";

            try
            {
                _context.Boats.Add(Boat);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    ModelState.AddModelError(string.Empty, "Erreur lors de l'ajout du bateau.");
                    LoadBoatTypes();
                    LoadPorts();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erreur lors de l'ajout du bateau : " + ex.Message);
                LoadBoatTypes();
                LoadPorts();
                return Page();
            }

            return RedirectToPage("/Index");
        }

        private void LoadBoatTypes()
        {
            if (!_context.BoatTypes.Any())
            {
                var defaultTypes = new List<BoatType>
                {
                    new BoatType { Name = "Voilier", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e5/Sailboat_icon.svg/1024px-Sailboat_icon.svg.png" },
                    new BoatType { Name = "Yacht", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/19/Yacht_icon.svg/1024px-Yacht_icon.svg.png" },
                    new BoatType { Name = "Péniche", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f5/Canal_Barge_icon.svg/1024px-Canal_Barge_icon.svg.png" },
                    new BoatType { Name = "Catamaran", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/90/Catamaran_icon.svg/1024px-Catamaran_icon.svg.png" },
                    new BoatType { Name = "Bateau à moteur", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/dc/Motorboat_icon.svg/1024px-Motorboat_icon.svg.png" }
                };

                _context.BoatTypes.AddRange(defaultTypes);
                _context.SaveChanges();
            }

            BoatTypes = _context.BoatTypes
                .OrderBy(bt => bt.Name)
                .Select(bt => new SelectListItem
                {
                    Value = bt.Id.ToString(),
                    Text = bt.Name
                }).ToList();

            // Ajoute une option vide au début
            BoatTypes.Insert(0, new SelectListItem { Value = "", Text = "-- Sélectionnez un type --" });
        }

        private void LoadPorts()
        {
            Ports = _context.Ports
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();

            // Ajoute une option vide au début
            Ports.Insert(0, new SelectListItem { Value = "", Text = "-- Sélectionnez un port --" });
        }
    }
}
