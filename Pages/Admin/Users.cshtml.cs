using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaillingLoc.Pages.Admin
{
    // Classe ViewModel pour afficher les utilisateurs avec leurs rôles
    public class UserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class UsersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UsersModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<UserViewModel> Users { get; set; } = new();

        // Statistiques
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int TenantCount { get; set; }
        public int OwnerCount { get; set; }
        public int AdminCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty]
        public User EditedUser { get; set; }

        [BindProperty]
        public string UserIdToDelete { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(u =>
                    u.FirstName.Contains(SearchTerm) ||
                    u.LastName.Contains(SearchTerm) ||
                    u.Email.Contains(SearchTerm));
            }

            var users = await query.ToListAsync();

            // Convertir les utilisateurs en UserViewModel avec leurs rôles
            Users = new List<UserViewModel>();
            
            // Initialiser les compteurs
            TenantCount = 0;
            OwnerCount = 0;
            AdminCount = 0;

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Users.Add(new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList()
                });

                // Compter les rôles
                foreach (var role in roles)
                {
                    if (role.Equals("Locataire", StringComparison.OrdinalIgnoreCase))
                        TenantCount++;
                    else if (role.Equals("Propriétaire", StringComparison.OrdinalIgnoreCase) || 
                             role.Equals("Proprietaire", StringComparison.OrdinalIgnoreCase))
                        OwnerCount++;
                    else if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
                             role.Equals("Administrateur", StringComparison.OrdinalIgnoreCase))
                        AdminCount++;
                }
            }

            // Calculer les statistiques
            TotalUsers = Users.Count;
            ActiveUsers = TotalUsers; // Vous pouvez ajouter une logique pour déterminer les utilisateurs actifs
            InactiveUsers = 0; // À implémenter selon votre logique métier
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var user = await _context.Users.FindAsync(EditedUser.Id);
            if (user == null)
                return NotFound();

            user.FirstName = EditedUser.FirstName;
            user.LastName = EditedUser.LastName;
            user.Email = EditedUser.Email;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToPage(new { searchTerm = SearchTerm });
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var user = await _context.Users.FindAsync(UserIdToDelete);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToPage(new { searchTerm = SearchTerm });
        }
    }
}