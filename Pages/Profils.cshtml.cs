using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using SaillingLoc.Data;
using SaillingLoc.Models;

namespace SailingLoc.Pages
{
    public class ProfilesModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public ProfilesModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public User CurrentUser { get; set; }
        public string FullName { get; set; }

        public async Task OnGetAsync()
        {
            CurrentUser = await _userManager.GetUserAsync(User);

            // Si l'utilisateur n'est pas connecté ou introuvable, 
            // on peut créer un utilisateur de test pour l'affichage
            if (CurrentUser == null)
            {
                // Données de test pour le développement
                CurrentUser = new User
                {
                    // FullName = "william nthiekam",
                    // Email = "williamnthiekam392@gmail.com",
                    // PhoneNumber = "0615013694",
                    // Address = "Avenue William Grisard, 7"
                };
            }
        }
    }
}