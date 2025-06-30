using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaillingLoc.Data;
using SaillingLoc.Models;

namespace SaillingLoc.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Liste des bateaux à afficher sur la page d'accueil
        public List<Boat> Boats { get; set; }

        // Liste déroulante des ports (pas utilisée dans la vue, mais prête pour un futur filtre)
        public IEnumerable<SelectListItem> PortOptions { get; set; }

        // Chargement des données lors d'un GET
        public async Task OnGetAsync()
        {
            Boats = await _context.Boats
                .Include(b => b.Port)         // Inclure les ports liés
                .Include(b => b.BoatType)     // Inclure le type de bateau
                .Include(b => b.Photos)       // Inclure les photos pour l'affichage
                .ToListAsync();

            var ports = await _context.Ports.ToListAsync();
            PortOptions = ports.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });
        }

        // Suppression d'un bateau (POST)
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var boat = await _context.Boats.FindAsync(id);
            if (boat == null)
            {
                return NotFound();
            }

            _context.Boats.Remove(boat);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
