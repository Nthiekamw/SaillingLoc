







using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SaillingLoc.Data;
using SaillingLoc.Models;
using SaillingLoc.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SaillingLoc.Pages.Proprietaire
{
    [Authorize(Roles = "Admin,Proprietaire")]
    public class AjoutBatModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;
        private readonly INotificationService _notificationService;

        public AjoutBatModel(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            UserManager<User> userManager,
            INotificationService notificationService)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        [BindProperty]
        public Boat Boat { get; set; } = new();

        [BindProperty]
        public IFormFile Photo { get; set; }

        public List<SelectListItem> BoatTypes { get; set; } = new();
        public List<SelectListItem> Ports { get; set; } = new();

        public void OnGet()
        {
            LoadBoatTypes();
            LoadPorts();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Boat.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(Boat.UserId))
{
    ModelState.Remove("Boat.UserId");
}
else
{
    ModelState.AddModelError(string.Empty, "Utilisateur non identifié.");
}

            if (Photo == null || Photo.Length == 0)
                ModelState.AddModelError("Photo", "Une photo est obligatoire.");

            if (Boat.BoatTypeId == 0)
                ModelState.AddModelError("Boat.BoatTypeId", "Le type de bateau est obligatoire.");

            if (Boat.PortId == 0)
                ModelState.AddModelError("Boat.PortId", "Le port est obligatoire.");

            if (!ModelState.IsValid)
            {
                LoadBoatTypes();
                LoadPorts();
                return Page();
            }

            Boat.CreatedAt = DateTime.UtcNow;
            Boat.UpdatedAt = DateTime.UtcNow;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            if (Photo != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(Photo.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    ModelState.AddModelError("Photo", "Seules les images JPG, JPEG ou PNG sont autorisées.");

                if (Photo.Length > 5 * 1024 * 1024)
                    ModelState.AddModelError("Photo", "La photo ne doit pas dépasser 5 Mo.");
            }

            if (!ModelState.IsValid)
            {
                LoadBoatTypes();
                LoadPorts();
                return Page();
            }

            if (Photo != null)
            {
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(Photo.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(stream);
                }

                Boat.Photo = $"/uploads/{uniqueFileName}";
            }
            else
            {
                Boat.Photo = "/images/default-boat.jpg";
            }

            try
            {
                _context.Boats.Add(Boat);
                await _context.SaveChangesAsync();

                // 🔔 Envoyer notifications aux autres utilisateurs
                await _notificationService.NotifyAllUsersNewBoatAsync(Boat.Name);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erreur lors de l'ajout du bateau ou de la notification : {ex.Message}");
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
                    new BoatType { Name = "Voilier", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/e/e5/Sailboat_icon.svg" },
                    new BoatType { Name = "Yacht", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/1/19/Yacht_icon.svg" },
                    new BoatType { Name = "Péniche", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/f/f5/Canal_Barge_icon.svg" },
                    new BoatType { Name = "Catamaran", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/9/90/Catamaran_icon.svg" },
                    new BoatType { Name = "Bateau à moteur", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/d/dc/Motorboat_icon.svg" }
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

            Ports.Insert(0, new SelectListItem { Value = "", Text = "-- Sélectionnez un port --" });
        }
    }
}































// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using SaillingLoc.Data;
// using SaillingLoc.Models;
// using SaillingLoc.Services;
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Security.Claims;
// using System.Threading.Tasks;

// namespace SaillingLoc.Pages.Proprietaire
// {
//     [Authorize(Roles = "Admin,Proprietaire")]
//     public class AjoutBatModel : PageModel
//     {
//         private readonly ApplicationDbContext _context;
//         private readonly IWebHostEnvironment _env;
//         private readonly UserManager<User> _userManager;
//         private readonly IEmailService _emailService;

//         public AjoutBatModel(ApplicationDbContext context, IWebHostEnvironment env, UserManager<User> userManager, IEmailService emailService)
//         {
//             _context = context;
//             _env = env;
//             _userManager = userManager;
//             _emailService = emailService;
//         }

//         [BindProperty]
//         public Boat Boat { get; set; } = new();

//         [BindProperty]
//         public IFormFile Photo { get; set; }

//         public List<SelectListItem> BoatTypes { get; set; } = new();
//         public List<SelectListItem> Ports { get; set; } = new();

//         public void OnGet()
//         {
//             LoadBoatTypes();
//             LoadPorts();
//         }

//         public async Task<IActionResult> OnPostAsync()
//         {
//             Boat.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//             if (string.IsNullOrEmpty(Boat.UserId))
//                 ModelState.AddModelError(string.Empty, "Utilisateur non identifié.");

//             if (Photo == null || Photo.Length == 0)
//                 ModelState.AddModelError("Photo", "Une photo est obligatoire.");

//             if (Boat.BoatTypeId == 0)
//                 ModelState.AddModelError("Boat.BoatTypeId", "Le type de bateau est obligatoire.");

//             if (Boat.PortId == 0)
//                 ModelState.AddModelError("Boat.PortId", "Le port est obligatoire.");

//             if (!ModelState.IsValid)
//             {
//                 LoadBoatTypes();
//                 LoadPorts();
//                 return Page();
//             }

//             Boat.CreatedAt = DateTime.UtcNow;
//             Boat.UpdatedAt = DateTime.UtcNow;

//             var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
//             if (!Directory.Exists(uploadsFolder))
//                 Directory.CreateDirectory(uploadsFolder);

//             if (Photo != null)
//             {
//                 var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
//                 var extension = Path.GetExtension(Photo.FileName).ToLower();

//                 if (!allowedExtensions.Contains(extension))
//                     ModelState.AddModelError("Photo", "Seules les images JPG, JPEG ou PNG sont autorisées.");

//                 if (Photo.Length > 5 * 1024 * 1024)
//                     ModelState.AddModelError("Photo", "La photo ne doit pas dépasser 5 Mo.");
//             }

//             if (!ModelState.IsValid)
//             {
//                 LoadBoatTypes();
//                 LoadPorts();
//                 return Page();
//             }

//             if (Photo != null)
//             {
//                 var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(Photo.FileName)}";
//                 var filePath = Path.Combine(uploadsFolder, uniqueFileName);

//                 using (var stream = new FileStream(filePath, FileMode.Create))
//                 {
//                     await Photo.CopyToAsync(stream);
//                 }

//                 Boat.Photo = $"/uploads/{uniqueFileName}";
//             }
//             else
//             {
//                 Boat.Photo = "/images/default-boat.jpg";
//             }

//             try
//             {
//                 _context.Boats.Add(Boat);
//                 await _context.SaveChangesAsync();
//             }
//             catch (Exception ex)
//             {
//                 ModelState.AddModelError(string.Empty, $"Erreur lors de l'ajout du bateau : {ex.Message}");
//                 LoadBoatTypes();
//                 LoadPorts();
//                 return Page();
//             }

//             // 🔔 Notifications + emails
//             try
//             {
//                 var currentUserId = Boat.UserId;
//                 var currentUser = await _userManager.GetUserAsync(User);

//                 var otherUsers = _context.Users
//                     .Where(u => u.Id != currentUserId)
//                     .ToList();

//                 foreach (var user in otherUsers)
//                 {
//                     _context.Notifications.Add(new Notification
//                     {
//                         UserId = user.Id,
//                         Message = $"Un nouveau bateau a été ajouté par {currentUser.UserName ?? currentUser.Email}.",
//                         CreatedAt = DateTime.UtcNow,
//                         IsRead = false
//                     });

//                     await _emailService.SendEmailAsync(
//                         user.Email,
//                         "Nouveau bateau disponible",
//                         $"Bonjour {user.UserName},<br/>Un nouveau bateau a été ajouté par {currentUser.UserName ?? currentUser.Email}."
//                     );
//                 }

//                 await _context.SaveChangesAsync();
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine("Erreur lors de l'envoi des notifications ou emails : " + ex.Message);
//             }

//             return RedirectToPage("/Index");
//         }

//         private void LoadBoatTypes()
//         {
//             if (!_context.BoatTypes.Any())
//             {
//                 var defaultTypes = new List<BoatType>
//                 {
//                     new BoatType { Name = "Voilier", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/e/e5/Sailboat_icon.svg" },
//                     new BoatType { Name = "Yacht", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/1/19/Yacht_icon.svg" },
//                     new BoatType { Name = "Péniche", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/f/f5/Canal_Barge_icon.svg" },
//                     new BoatType { Name = "Catamaran", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/9/90/Catamaran_icon.svg" },
//                     new BoatType { Name = "Bateau à moteur", PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/d/dc/Motorboat_icon.svg" }
//                 };

//                 _context.BoatTypes.AddRange(defaultTypes);
//                 _context.SaveChanges();
//             }

//             BoatTypes = _context.BoatTypes
//                 .OrderBy(bt => bt.Name)
//                 .Select(bt => new SelectListItem
//                 {
//                     Value = bt.Id.ToString(),
//                     Text = bt.Name
//                 }).ToList();

//             BoatTypes.Insert(0, new SelectListItem { Value = "", Text = "-- Sélectionnez un type --" });
//         }

//         private void LoadPorts()
//         {
//             Ports = _context.Ports
//                 .OrderBy(p => p.Name)
//                 .Select(p => new SelectListItem
//                 {
//                     Value = p.Id.ToString(),
//                     Text = p.Name
//                 }).ToList();

//             Ports.Insert(0, new SelectListItem { Value = "", Text = "-- Sélectionnez un port --" });
//         }
//     }
// }
