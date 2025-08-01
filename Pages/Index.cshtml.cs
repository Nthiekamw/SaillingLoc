using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaillingLoc.Data;
using SaillingLoc.Models;

namespace SaillingLoc.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public List<Boat> Boats { get; set; }
        public IEnumerable<SelectListItem> PortOptions { get; set; }

        public string CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsProprietaire { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchDestination { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public DateTime? searchStartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? searchEndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? searchNumberOfPeople { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            CurrentUserId = user?.Id;
            IsAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");
            IsProprietaire = user != null && await _userManager.IsInRoleAsync(user, "Proprietaire");

            var query = _context.Boats
                .Include(b => b.Port)
                .Include(b => b.BoatType)
                .Include(b => b.Photos)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchDestination))
            {
                query = query.Where(b => b.Port != null && b.Port.Name.Contains(searchDestination));
            }

            if (searchNumberOfPeople.HasValue)
            {
                query = query.Where(b => b.Capacity >= searchNumberOfPeople.Value);
            }

            if (searchStartDate.HasValue && searchEndDate.HasValue)
            {
                query = query.Where(b => !_context.Reservations.Any(r =>
                    r.BoatId == b.Id &&
                    (
                        (searchStartDate.Value >= r.StartDate && searchStartDate.Value <= r.EndDate) ||
                        (searchEndDate.Value >= r.StartDate && searchEndDate.Value <= r.EndDate) ||
                        (searchStartDate.Value <= r.StartDate && searchEndDate.Value >= r.EndDate)
                    )
                ));
            }

            Boats = await query.ToListAsync();

            var ports = await _context.Ports.ToListAsync();
            PortOptions = ports.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var boat = await _context.Boats.FindAsync(id);
            if (boat == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Forbid();
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // Vérifie si l'utilisateur est admin ou propriétaire du bateau
            if (!isAdmin && boat.OwnerId != user.Id)
            {
                return Forbid(); // interdit la suppression
            }

            _context.Boats.Remove(boat);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}















































































// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using SaillingLoc.Data;
// using SaillingLoc.Models;

// namespace SaillingLoc.Pages
// {
//     public class IndexModel : PageModel
//     {
//         private readonly ILogger<IndexModel> _logger;
//         private readonly ApplicationDbContext _context;

//         public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
//         {
//             _logger = logger;
//             _context = context;
//         }

//         public List<Boat> Boats { get; set; }
//         public Boat Boat { get; set; }
//         public BoatPhoto BoatPhoto { get; set; }

//         public string GoogleMapsApiKey { get; set; } = "AIzaSyAv5i7Pcfoy7KGhEo3bs_NpVhxPj9JaUJs";

//         public IEnumerable<SelectListItem> PortOptions { get; set; }

//         // Bind propriétés avec noms exacts
//         [BindProperty(SupportsGet = true)]
//         public string searchDestination { get; set; } = "";

//         [BindProperty(SupportsGet = true)]
//         public DateTime? searchStartDate { get; set; }

//         [BindProperty(SupportsGet = true)]
//         public DateTime? searchEndDate { get; set; }

//         [BindProperty(SupportsGet = true)]
//         public int? searchNumberOfPeople { get; set; }

//         public async Task OnGetAsync()
//         {
//             var query = _context.Boats
//                 .Include(b => b.Port)
//                 .Include(b => b.BoatType)
//                 .Include(b => b.Photos)
//                 .AsQueryable();

//             if (!string.IsNullOrEmpty(searchDestination))
//             {
//                 query = query.Where(b => b.Port != null && b.Port.Name.Contains(searchDestination));
//             }

//             if (searchNumberOfPeople.HasValue)
//             {
//                 query = query.Where(b => b.Capacity >= searchNumberOfPeople.Value);
//             }

//             if (searchStartDate.HasValue && searchEndDate.HasValue)
//             {
//                 query = query.Where(b => !_context.Reservations.Any(r =>
//                     r.BoatId == b.Id &&
//                     (
//                         (searchStartDate.Value >= r.StartDate && searchStartDate.Value <= r.EndDate) ||
//                         (searchEndDate.Value >= r.StartDate && searchEndDate.Value <= r.EndDate) ||
//                         (searchStartDate.Value <= r.StartDate && searchEndDate.Value >= r.EndDate)
//                     )
//                 ));
//             }

//             Boats = await query.ToListAsync();

//             var ports = await _context.Ports.ToListAsync();
//             PortOptions = ports.Select(p => new SelectListItem
//             {
//                 Value = p.Id.ToString(),
//                 Text = p.Name
//             });
//         }

//         public async Task<IActionResult> OnPostDeleteAsync(int id)
//         {
//             var boat = await _context.Boats.FindAsync(id);
//             if (boat == null)
//             {
//                 return NotFound();
//             }

//             _context.Boats.Remove(boat);
//             await _context.SaveChangesAsync();

//             return RedirectToPage();
//         }
//     }
// }





















// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using SaillingLoc.Data;
// using SaillingLoc.Models;

// namespace SaillingLoc.Pages
// {
//     public class IndexModel : PageModel
//     {
//         private readonly ILogger<IndexModel> _logger;
//         private readonly ApplicationDbContext _context;

//         public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
//         {
//             _logger = logger;
//             _context = context;
//         }

//         // Liste des bateaux à afficher sur la page d'accueil
//         public List<Boat> Boats { get; set; }
//         public Boat Boat { get; set; }
//         public BoatPhoto BoatPhoto { get; set; }

//         // Clé API Google Maps
//         public string GoogleMapsApiKey { get; set; } = "AIzaSyAv5i7Pcfoy7KGhEo3bs_NpVhxPj9JaUJs";

//         // Liste déroulante des ports (non utilisée mais disponible)
//         public IEnumerable<SelectListItem> PortOptions { get; set; }

//         // Chargement des données lors d'un GET
//         public async Task OnGetAsync()
//         {
//             Boats = await _context.Boats
//                 .Include(b => b.Port)         // Inclure les ports liés
//                 .Include(b => b.BoatType)     // Inclure le type de bateau
//                 .Include(b => b.Photos)       // Inclure les photos pour l'affichage
//                 .ToListAsync();

//             var ports = await _context.Ports.ToListAsync();
//             PortOptions = ports.Select(p => new SelectListItem
//             {
//                 Value = p.Id.ToString(),
//                 Text = p.Name
//             });
//         }

//         // Suppression d'un bateau (POST)
//         public async Task<IActionResult> OnPostDeleteAsync(int id)
//         {
//             var boat = await _context.Boats.FindAsync(id);
//             if (boat == null)
//             {
//                 return NotFound();
//             }

//             _context.Boats.Remove(boat);
//             await _context.SaveChangesAsync();

//             return RedirectToPage();
//         }
//     }
// }
