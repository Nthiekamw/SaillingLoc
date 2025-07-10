using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SaillingLoc.Models;
using System.Threading.Tasks;

public class MyPageModel : PageModel
{
    private readonly IUserActionLogger _logger;
    private readonly UserManager<User> _userManager;

    public MyPageModel(IUserActionLogger logger, UserManager<User> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        await _logger.LogAsync("Consultation de la page", "/MyPage", user?.Id, ip);

        return Page();
    }
}
