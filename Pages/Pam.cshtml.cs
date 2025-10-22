using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace SailingLoc.Pages
{
    public class PamModel : PageModel
    {
        [BindProperty]
        public bool NotificationsEmail { get; set; }

        [BindProperty]
        public bool Newsletter { get; set; }

        [BindProperty]
        public string EmailVisibility { get; set; }

        public List<string> UserRoles { get; set; }

        public void OnGet()
        {
            // Initialiser les paramètres de l'utilisateur
            // Dans une vraie application, récupérer depuis la base de données
            NotificationsEmail = true;
            Newsletter = true;
            EmailVisibility = "Prive";

            // Rôles de l'utilisateur
            UserRoles = new List<string> { "Locataire", "Propriétaire" };
        }

        public IActionResult OnPostUpdateNotifications()
        {
            // TODO: Sauvegarder les préférences de notification dans la base de données
            // Exemple:
            // var user = _context.Users.Find(GetCurrentUserId());
            // user.NotificationsEmail = NotificationsEmail;
            // user.Newsletter = Newsletter;
            // _context.SaveChanges();

            TempData["SuccessMessage"] = "Préférences de notification mises à jour.";
            return RedirectToPage();
        }

        public IActionResult OnPostUpdatePrivacy()
        {
            // TODO: Sauvegarder les paramètres de confidentialité dans la base de données
            // Exemple:
            // var user = _context.Users.Find(GetCurrentUserId());
            // user.EmailVisibility = EmailVisibility;
            // _context.SaveChanges();

            TempData["SuccessMessage"] = "Paramètres de confidentialité mis à jour.";
            return RedirectToPage();
        }

        public IActionResult OnPostAddRole(string role)
        {
            // TODO: Ajouter un nouveau rôle à l'utilisateur
            // Exemple:
            // var userRole = new UserRole
            // {
            //     UserId = GetCurrentUserId(),
            //     Role = role,
            //     IsVerified = false
            // };
            // _context.UserRoles.Add(userRole);
            // _context.SaveChanges();

            TempData["SuccessMessage"] = $"Rôle '{role}' ajouté. Une vérification peut être nécessaire.";
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveRole(string role)
        {
            // TODO: Retirer un rôle de l'utilisateur
            // Exemple:
            // var userRole = _context.UserRoles
            //     .FirstOrDefault(ur => ur.UserId == GetCurrentUserId() && ur.Role == role);
            // if (userRole != null)
            // {
            //     _context.UserRoles.Remove(userRole);
            //     _context.SaveChanges();
            // }

            TempData["SuccessMessage"] = $"Rôle '{role}' supprimé.";
            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            // TODO: Implémenter la déconnexion
            // Exemple avec Identity:
            // await _signInManager.SignOutAsync();

            return RedirectToPage("/Index");
        }

        public IActionResult OnPostDeleteAccount()
        {
            // TODO: Supprimer le compte utilisateur
            // ATTENTION: Cette action doit être irréversible et bien confirmée
            // Exemple:
            // var user = _context.Users.Find(GetCurrentUserId());
            // if (user != null)
            // {
            //     // Supprimer toutes les données associées
            //     _context.Users.Remove(user);
            //     _context.SaveChanges();
            //     
            //     // Déconnecter l'utilisateur
            //     await _signInManager.SignOutAsync();
            // }

            TempData["SuccessMessage"] = "Compte supprimé avec succès.";
            return RedirectToPage("/Index");
        }

        // Méthode helper pour obtenir l'ID de l'utilisateur connecté
        private int GetCurrentUserId()
        {
            // TODO: Récupérer l'ID de l'utilisateur connecté
            // Exemple avec Identity:
            // return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return 1; // Valeur temporaire pour les tests
        }
    }
}