using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SaillingLoc.Data;
using SaillingLoc.Models;
using SaillingLoc.Services;
using SaillingLoc.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.IO;
using Microsoft.Extensions.FileProviders;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // === Configuration du DbContext avec SQL Server ===
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // === Configuration d'Identity avec rôles et User personnalisé ===
        builder.Services.AddIdentity<User, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false; // À activer en production
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();



// Activer la politique de cookies
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // Nécessité du consentement pour les cookies non essentiels
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});







        // === Services personnalisés ===
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IUserActionLogger, UserActionLogger>();


        // === SignalR ===
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<IUserIdProvider, IdentifierUserIdProvider>();

        // === Razor Pages & Controllers ===
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();

        var app = builder.Build();
        using (var scope = app.Services.CreateScope()) 
        { 
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); dbContext.Database.Migrate();
        }

        // === Initialisation rôles + utilisateur admin ===
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                Console.WriteLine("▶ Initialisation des rôles...");
                await RoleInitializer.InitializeRolesAsync(services);
                Console.WriteLine("▶ Création de l'utilisateur admin...");
                await RoleInitializer.CreateAdminUserAsync(services);
                Console.WriteLine("✅ Initialisation terminée.");
                 await RoleInitializer.InitializePortsAsync(services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de l'initialisation : {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"→ Détails : {ex.InnerException.Message}");
            }
        }

        // === Middlewares ===
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

//  HEAD
        // Middleware pour la politique de cookies
app.UseCookiePolicy();

        app.UseStatusCodePagesWithReExecute("/NotFound");

        app.UseStaticFiles();

        
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "wwwroot"))
        });
// f45ed24323b2c8fb70ad0d5f865c17a857694b57

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        // === Mapping des routes ===
        app.MapRazorPages();
        app.MapControllers();
        app.MapHub<NotificationHub>("/notificationHub");

        app.Run();
    }
}

// =====================================
// Classe statique pour initialiser rôles et utilisateur admin
// =====================================
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
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                    Console.WriteLine($"✅ Rôle '{roleName}' créé.");
                else
                    Console.WriteLine($"❌ Échec création rôle '{roleName}' : {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            else
            {
                Console.WriteLine($"ℹ️ Rôle '{roleName}' existe déjà.");
            }
        }
    }

    public static async Task CreateAdminUserAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        const string adminEmail = "admin@loc.com";
        const string adminPassword = "Admin123!"; // ⚠️ À sécuriser
        var existingUser = await userManager.FindByEmailAsync(adminEmail);
        if (existingUser == null)
        {
            var adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "Root",
                Address = "Adresse par défaut",
                PhoneNumber = "0000000000",
                PaymentMethod = "None",
                Photo = "",
                Reference = "N/A",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine("✅ Utilisateur admin créé.");
            }
            else
            {
                HandleErrors(result.Errors, "création de l'utilisateur admin");
            }
        }
        else
        {
            Console.WriteLine("ℹ️ L'utilisateur admin existe déjà.");
        }
    }

    private static void HandleErrors(IEnumerable<IdentityError> errors, string action)
    {
        Console.WriteLine($"❌ Échec de {action} :");
        foreach (var error in errors)
        {
            Console.WriteLine($"- {error.Description}");
        }
    }

 public static async Task InitializePortsAsync(IServiceProvider serviceProvider)
{
     var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

     // Vérifie si des ports existent déjà pour éviter les doublons
  if (dbContext.Ports.Any())
    {
        Console.WriteLine("ℹ️ Les ports sont déjà initialisés.");
        return;
    }

   var ports = new List<Port>
   {
        new Port
        {
           Name = "Port de Toulon",
           City = "Toulon",
            Country = "France",
            Latitude = 43.1242,
            Longitude = 5.9280,
           CreatedAt = new DateTime(2025, 6, 28, 20, 59, 41, 586, DateTimeKind.Utc).AddTicks(9557),
            UpdatedAt = new DateTime(2025, 6, 28, 20, 59, 41, 586, DateTimeKind.Utc).AddTicks(9558)
        },
         new Port
        {
            Name = "Port de Nice",
             City = "Nice",
            Country = "France",
             Latitude = 43.7102,
             Longitude = 7.2620,
             CreatedAt = new DateTime(2025, 6, 28, 20, 59, 41, 587, DateTimeKind.Utc).AddTicks(123),
             UpdatedAt = new DateTime(2025, 6, 28, 20, 59, 41, 587, DateTimeKind.Utc).AddTicks(123)
        },
        new Port
        {
            Name = "Port de Marseille",
            City = "Marseille",
            Country = "France",
            Latitude = 43.2965,
            Longitude = 5.3698,
            CreatedAt = new DateTime(2025, 6, 28, 20, 59, 41, 587, DateTimeKind.Utc).AddTicks(125),
            UpdatedAt = new DateTime(2025, 6, 28, 20, 59, 41, 587, DateTimeKind.Utc).AddTicks(125)
       }
    };
     await dbContext.Ports.AddRangeAsync(ports);
     await dbContext.SaveChangesAsync();

     Console.WriteLine("✅ Ports initialisés avec succès.");
 }
        



}



