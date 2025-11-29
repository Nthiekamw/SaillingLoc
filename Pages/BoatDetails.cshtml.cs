
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System;
using System.Threading.Tasks;

namespace SaillingLoc.Pages
{
    public class BoatDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public BoatDetailsModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Boat Boat { get; set; }
        public bool CanEditEquipment { get; set; }

        [BindProperty]
        public string NewEquipmentName { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Boat = await _context.Boats
                .Include(b => b.Photos)
                .Include(b => b.Port)
                .Include(b => b.BoatType)
                .Include(b => b.Equipments)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (Boat == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            // Seul l'admin ou le propriétaire du bateau peut éditer les équipements
            CanEditEquipment =
                User.IsInRole("Admin") ||
                (currentUser != null && Boat.OwnerId == currentUser.Id);

            return Page();
        }

        public async Task<IActionResult> OnPostAddEquipmentAsync(int id)
        {
            var boat = await _context.Boats.FirstOrDefaultAsync(b => b.Id == id);
            if (boat == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            // Vérifier que l'utilisateur est soit l'admin soit le propriétaire du bateau
            bool canEdit =
                User.IsInRole("Admin") ||
                (currentUser != null && boat.OwnerId == currentUser.Id);

            if (!canEdit)
                return Forbid();

            if (!string.IsNullOrWhiteSpace(NewEquipmentName))
            {
                var equipment = new BoatEquipment
                {
                    BoatId = boat.Id,
                    EquipmentName = NewEquipmentName.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.BoatEquipments.Add(equipment);
                boat.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id });
        }
    }
}





















































































































// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using Microsoft.EntityFrameworkCore;
// using SaillingLoc.Data;
// using SaillingLoc.Models;
// using System;
// using System.Threading.Tasks;

// namespace SaillingLoc.Pages   // ← IMPORTANT !
// {
//     public class BoatDetailsModel : PageModel
//     {
//         private readonly ApplicationDbContext _context;
//         private readonly UserManager<User> _userManager;

//         public BoatDetailsModel(ApplicationDbContext context, UserManager<User> userManager)
//         {
//             _context = context;
//             _userManager = userManager;
//         }

//         public Boat Boat { get; set; }
//         public bool CanEditEquipment { get; set; }

//         [BindProperty]
//         public string NewEquipmentName { get; set; }

//         public async Task<IActionResult> OnGetAsync(int id)
//         {
//             Boat = await _context.Boats
//                 .Include(b => b.Photos)
//                 .Include(b => b.Port)
//                 .Include(b => b.BoatType)
//                 .Include(b => b.Equipments)
//                 .FirstOrDefaultAsync(b => b.Id == id);

//             if (Boat == null)
//                 return NotFound();

//             var currentUser = await _userManager.GetUserAsync(User);

//             CanEditEquipment =
//                 User.IsInRole("Admin") ||
//                 (currentUser != null && Boat.OwnerId == currentUser.Id);

//             return Page();
//         }

//         public async Task<IActionResult> OnPostAddEquipmentAsync(int id)
//         {
//             var boat = await _context.Boats.FirstOrDefaultAsync(b => b.Id == id);
//             if (boat == null)
//                 return NotFound();

//             var currentUser = await _userManager.GetUserAsync(User);

//             bool canEdit =
//                 User.IsInRole("Admin") ||
//                 (currentUser != null && boat.OwnerId == currentUser.Id);

//             if (!canEdit)
//                 return Forbid();

//             if (!string.IsNullOrWhiteSpace(NewEquipmentName))
//             {
//                 var equipment = new BoatEquipment
//                 {
//                     BoatId = boat.Id,
//                     EquipmentName = NewEquipmentName.Trim(),
//                     CreatedAt = DateTime.UtcNow,
//                     UpdatedAt = DateTime.UtcNow
//                 };

//                 _context.BoatEquipments.Add(equipment);
//                 boat.UpdatedAt = DateTime.UtcNow;

//                 await _context.SaveChangesAsync();
//             }

//             return RedirectToPage(new { id });
//         }
//     }
// }




















































// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using Microsoft.EntityFrameworkCore;
// using SaillingLoc.Data;
// using SaillingLoc.Models;
// using System.Threading.Tasks;

// namespace SaillingLoc.Pages
// {
//     public class BoatDetailsModel : PageModel
//     {
//         private readonly ApplicationDbContext _context;

//         public BoatDetailsModel(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//         [BindProperty]
//         public Boat Boat { get; set; }

//         public async Task<IActionResult> OnGetAsync(int id)
//         {
//             Boat = await _context.Boats
//                 .Include(b => b.Photos)
//                 .Include(b => b.Port)
//                 .Include(b => b.BoatType)
//                 .FirstOrDefaultAsync(b => b.Id == id);

//             if (Boat == null)
//             {
//                 return NotFound();
//             }

//             return Page();
//         }
//     }
// }
