using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaillingLoc.Data;
using SaillingLoc.Models;

namespace SaillingLoc.Pages
{
    public class BateauxModel : PageModel
    {
        private readonly ILogger<BateauxModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private const int PageSize = 9; // 9 bateaux par page

        public BateauxModel(ILogger<BateauxModel> logger, ApplicationDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }


        [BindProperty(SupportsGet = true)]
public string? LieuDepart { get; set; }

[BindProperty(SupportsGet = true)]
public DateTime? DateDepart { get; set; }

[BindProperty(SupportsGet = true)]
public DateTime? DateArrivee { get; set; }

[BindProperty(SupportsGet = true)]
public string? TypeBateau { get; set; }


        // Liste des bateaux affichés
        public List<Boat> Boats { get; set; }

        // Liste des ports pour le select
        public SelectList PortsList { get; set; }

        // Infos utilisateur courant
        public string CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsProprietaire { get; set; }

        // Filtres bindés
        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedBoatType { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedPortId { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        // Pagination
        public int TotalPages { get; set; }

       public async Task OnGetAsync()
{
    // 🔹 1. Récupérer l'utilisateur connecté et ses rôles
    var user = await _userManager.GetUserAsync(User);
    CurrentUserId = user?.Id;
    IsAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");
    IsProprietaire = user != null && await _userManager.IsInRoleAsync(user, "Proprietaire");

    // 🔹 2. Charger la liste des ports pour le filtre
    var ports = await _context.Ports.OrderBy(p => p.Name).ToListAsync();
    PortsList = new SelectList(ports, "Id", "Name", SelectedPortId);

    // 🔹 3. Construire la requête de base
    var query = _context.Boats
        .Include(b => b.Port)
        .Include(b => b.BoatType)
        .Include(b => b.Photos)
        .Include(b => b.Reservations) // nécessaire pour vérifier la dispo
        .AsQueryable();

    // 🔹 4. Appliquer les filtres standards
    if (!string.IsNullOrEmpty(SearchQuery))
    {
        query = query.Where(b =>
            b.Name.Contains(SearchQuery) ||
            (b.Description != null && b.Description.Contains(SearchQuery)));
    }

    if (!string.IsNullOrEmpty(SelectedBoatType))
    {
        query = query.Where(b => b.BoatType != null && b.BoatType.Name == SelectedBoatType);
    }

    if (SelectedPortId.HasValue)
    {
        query = query.Where(b => b.PortId == SelectedPortId.Value);
    }

    if (MinPrice.HasValue)
    {
        query = query.Where(b => b.DailyPrice >= MinPrice.Value);
    }

    if (MaxPrice.HasValue)
    {
        query = query.Where(b => b.DailyPrice <= MaxPrice.Value);
    }

    // 🔹 5. Nouveau : Filtre de recherche depuis la page d'accueil (lieu + date)
    if (!string.IsNullOrEmpty(LieuDepart))
    {
        query = query.Where(b => b.Port.Name == LieuDepart);
    }

    if (DateDepart.HasValue && DateArrivee.HasValue)
    {
        // Filtrer les bateaux sans réservation sur la période donnée
        query = query.Where(b => !b.Reservations.Any(r =>
            (r.StartDate <= DateArrivee && r.EndDate >= DateDepart)
        ));
    }

    // 🔹 6. Pagination
    var totalBoats = await query.CountAsync();
    TotalPages = (int)Math.Ceiling(totalBoats / (double)PageSize);

    if (CurrentPage < 1) CurrentPage = 1;
    if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;

    Boats = await query
        .OrderByDescending(b => b.Id)
        .Skip((CurrentPage - 1) * PageSize)
        .Take(PageSize)
        .ToListAsync();

    // 🔹 7. Message si aucun bateau trouvé
    if (!Boats.Any())
    {
        TempData["Message"] = "Aucun bateau disponible pour cette date.";
    }
}

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var boat = await _context.Boats.FindAsync(id);
            if (boat == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin && boat.OwnerId != user.Id)
                return Forbid(); // interdit la suppression si pas propriétaire ni admin

            _context.Boats.Remove(boat);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Le bateau a été supprimé avec succès.";
            return RedirectToPage();
        }
    }
}
