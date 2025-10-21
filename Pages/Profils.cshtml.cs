using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using SaillingLoc.Data;
using SaillingLoc.Models;

namespace SaillingLoc.Pages
{
    public class ProfilesModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public ProfilesModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

   public User CurrentUser { get; set; }  // <-- renommée
        public User User { get; set; }

        public async Task OnGetAsync()
        {
            CurrentUser = await _userManager.GetUserAsync(base.User); // <-- "base.User" = ClaimsPrincipal
              User = CurrentUser; // <--- AJOUT
        }
    }
}