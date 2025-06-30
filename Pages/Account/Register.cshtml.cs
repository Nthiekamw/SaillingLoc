using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SaillingLoc.Models;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace SaillingLoc.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public RegisterModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Prénom")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Nom")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmer le mot de passe")]
            [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Veuillez sélectionner un rôle.")]
            [Display(Name = "Rôle")]
            public string Role { get; set; } // "Locataire" ou "Propriétaire"

            [Display(Name = "Photo de profil")]
            public IFormFile Photo { get; set; } // Pour le téléchargement de fichiers
        }

        public void OnGet()
        {
            // Rien à faire ici
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new User
            {
                UserName = Input.Email,
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Reference = Guid.NewGuid().ToString() // Assurez-vous d'initialiser Reference
            };

            // Gérer le téléchargement de la photo, si fourni
            if (Input.Photo != null)
            {
                var filePath = Path.Combine("wwwroot/uploads", Input.Photo.FileName); // Chemin de sauvegarde
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.Photo.CopyToAsync(stream);
                }
                user.Photo = $"/uploads/{Input.Photo.FileName}"; // Enregistrez le chemin relatif
            }

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Input.Role);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("/Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}