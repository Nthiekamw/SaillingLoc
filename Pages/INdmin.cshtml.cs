using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaillingLoc.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class INdminModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public INdminModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Statistiques
        public int UserCount { get; set; }
        public int AnnonceCount { get; set; }
        public int CommentCount { get; set; }
        public int ContratCount { get; set; }

        // Utilisateurs
        public List<User> Users { get; set; } = new();
        public Dictionary<string, IList<string>> UserRoles { get; set; } = new();

        // GET: Charger les données
        public async Task OnGetAsync()
        {
            UserCount = await _context.Users.CountAsync();
            AnnonceCount = await _context.Boats.CountAsync();
            CommentCount = await _context.Messages.CountAsync();
            ContratCount = await _context.Contracts.CountAsync();

            Users = await _userManager.Users.ToListAsync();

            foreach (var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserRoles[user.Id] = roles;
            }
        }

        // POST: Changer le rôle d’un utilisateur
        public async Task<IActionResult> OnPostChangeRoleAsync(string userId, string newRole)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newRole))
            {
                return RedirectToPage(); // Valeurs invalides
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(); // Utilisateur introuvable
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Erreur lors de la suppression des anciens rôles.");
                return RedirectToPage();
            }

            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Erreur lors de l'ajout du nouveau rôle.");
            }

            return RedirectToPage();
        }
    }
}
