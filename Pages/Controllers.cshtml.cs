using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SaillingLoc.Models;

[Route("init-admin")]
public class AdminInitController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminInitController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> Init()
    {
        string[] roles = { "Admin", "Proprietaire", "Locataire", "Visiteur" };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@loc.com";
        var adminPassword = "Admin123!";

        var admin = await _userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            var newAdmin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newAdmin, adminPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newAdmin, "Admin");
                return Ok("✅ Admin créé avec succès.");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        return Ok("ℹ️ Admin existe déjà.");
    }
}
