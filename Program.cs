using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models; // Pour le User
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Ajouter le DbContext avec SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajouter Identity avec rôles + User personnalisé
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Dev : false
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Ajouter Razor Pages
builder.Services.AddRazorPages();

// (Facultatif) API Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Initialisation des rôles au démarrage
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await RoleInitializer.InitializeRolesAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur lors de l'initialisation des rôles : {ex.Message}");
    }
}

// Middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Authentification
app.UseAuthorization();  // Autorisation

app.MapRazorPages();
app.MapControllers(); // Pour API

app.Run();


// ============================================
// Initialisation des rôles au démarrage
// ============================================
public static class RoleInitializer
{
    public static async Task InitializeRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "Admin", "Proprietaire", "Locataire", "Visiteur" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
