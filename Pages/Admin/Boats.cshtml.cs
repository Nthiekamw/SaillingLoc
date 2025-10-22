using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaillingLoc.Pages.Admin
{
    public class BoatsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public BoatsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Boat> Boats { get; set; } = new();
        public List<BoatType> BoatTypes { get; set; } = new();
        public List<Port> Ports { get; set; } = new();

        // Filtres
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string TypeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PortFilter { get; set; }

        // Statistiques
        public int TotalBoats { get; set; }
        public int ActiveBoats { get; set; }
        public int PendingBoats { get; set; }
        public double AverageRating { get; set; }

        public async Task OnGetAsync()
        {
            // Charger les options de filtres
            BoatTypes = await _context.BoatTypes.OrderBy(bt => bt.Name).ToListAsync();
            Ports = await _context.Ports.OrderBy(p => p.Name).ToListAsync();

            // Requête de base avec toutes les relations
            var query = _context.Boats
                .Include(b => b.BoatType)
                .Include(b => b.Port)
                .Include(b => b.Owner)
                .Include(b => b.User)
                .AsQueryable();

            // Appliquer le filtre de recherche
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(b =>
                    b.Name.Contains(SearchTerm) ||
                    b.Brand.Contains(SearchTerm) ||
                    b.Model.Contains(SearchTerm) ||
                    (b.Port != null && b.Port.Name.Contains(SearchTerm)) ||
                    (b.BoatType != null && b.BoatType.Name.Contains(SearchTerm))
                );
            }

            // Appliquer le filtre de statut
            if (!string.IsNullOrWhiteSpace(StatusFilter))
            {
                if (StatusFilter.Equals("actif", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(b => b.IsActive);
                }
                else if (StatusFilter.Equals("inactif", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(b => !b.IsActive);
                }
            }

            // Appliquer le filtre de type
            if (!string.IsNullOrWhiteSpace(TypeFilter) && int.TryParse(TypeFilter, out int typeId))
            {
                query = query.Where(b => b.BoatTypeId == typeId);
            }

            // Appliquer le filtre de port
            if (!string.IsNullOrWhiteSpace(PortFilter) && int.TryParse(PortFilter, out int portId))
            {
                query = query.Where(b => b.PortId == portId);
            }

            // Récupérer les bateaux filtrés
            Boats = await query
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            // Calculer les statistiques (sur tous les bateaux, pas seulement filtrés)
            var allBoats = await _context.Boats.ToListAsync();
            TotalBoats = allBoats.Count;
            ActiveBoats = allBoats.Count(b => b.IsActive);
            PendingBoats = allBoats.Count(b => !b.IsActive); // Ou selon votre logique métier
            AverageRating = 0.0; // TODO: Calculer la moyenne des notes si vous avez un système d'avis
        }

        // Méthode pour exporter en CSV
        public async Task<IActionResult> OnGetExportCsvAsync()
        {
            var boats = await _context.Boats
                .Include(b => b.BoatType)
                .Include(b => b.Port)
                .Include(b => b.Owner)
                .Include(b => b.User)
                .ToListAsync();

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Nom,Type,Port,Prix/Jour,Statut,Propriétaire");

            foreach (var boat in boats)
            {
                var owner = boat.Owner?.FullName ?? boat.User?.FullName ?? "Inconnu";
                var status = boat.IsActive ? "Actif" : "Inactif";
                csv.AppendLine($"{boat.Name},{boat.BoatType?.Name},{boat.Port?.Name},{boat.DailyPrice},{status},{owner}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"bateaux_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}