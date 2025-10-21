using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SaillingLoc.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SaillingLoc.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public RegisterModel(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Veuillez sélectionner un rôle.")]
            [Display(Name = "Rôle")]
            public string Role { get; set; } // "Locataire" ou "Propriétaire"

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

            [Phone]
            [Display(Name = "Téléphone")]
            public string? PhoneNumber { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmer le mot de passe")]
            [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Photo de profil")]
            public IFormFile? Photo { get; set; }
        }

        public class RecaptchaResponse
        {
            public bool success { get; set; }
            public string challenge_ts { get; set; }
            public string hostname { get; set; }
            public string[] error_codes { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // ✅ 1️⃣ Vérification du reCAPTCHA
            var captchaResponse = Request.Form["g-recaptcha-response"];
            var secretKey = _configuration["GoogleReCaptcha:SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                ModelState.AddModelError(string.Empty, "Erreur interne : clé reCAPTCHA manquante.");
                return Page();
            }

            using var client = new HttpClient();
            var response = await client.GetStringAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captchaResponse}");

            var captchaResult = JsonSerializer.Deserialize<RecaptchaResponse>(response);

            if (captchaResult == null || !captchaResult.success)
            {
                ModelState.AddModelError(string.Empty, "Veuillez valider le captcha avant de continuer.");
                return Page();
            }

            // ✅ 2️⃣ Création de l’utilisateur
            var user = new User
            {
                UserName = Input.Email,
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Reference = Guid.NewGuid().ToString(),
                PhoneNumber = Input.PhoneNumber,
            };

            // ✅ 3️⃣ Gestion du téléchargement de la photo
            if (Input.Photo != null)
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(Input.Photo.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.Photo.CopyToAsync(stream);
                }

                user.Photo = $"/uploads/{fileName}";
            }

            // ✅ 4️⃣ Création dans la base
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Input.Role);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("/Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }
    }
}
