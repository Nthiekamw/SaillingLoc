using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaillingLoc.Data;
using SaillingLoc.Models;

namespace SaillingLoc.Pages
{
    public class EditBoatModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditBoatModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Boat Boat { get; set; }

        public IEnumerable<SelectListItem> Ports { get; set; }
        public IEnumerable<SelectListItem> BoatTypes { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Boat = await _context.Boats.FindAsync(id);

            if (Boat == null)
                return NotFound();

            await LoadSelectListsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            var boatToUpdate = await _context.Boats.FindAsync(Boat.Id);

            if (boatToUpdate == null)
                return NotFound();

            // Mise à jour des propriétés modifiables
            boatToUpdate.Name = Boat.Name;
            boatToUpdate.DailyPrice = Boat.DailyPrice;
            boatToUpdate.PortId = Boat.PortId;
            boatToUpdate.BoatTypeId = Boat.BoatTypeId;
            boatToUpdate.Description = Boat.Description;

            // Si tu as d'autres propriétés modifiables, ajoute-les ici

            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }

        private async Task LoadSelectListsAsync()
        {
            var ports = await _context.Ports.ToListAsync();
            var boatTypes = await _context.BoatTypes.ToListAsync();

            Ports = ports.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name });
            BoatTypes = boatTypes.Select(bt => new SelectListItem { Value = bt.Id.ToString(), Text = bt.Name });
        }
    }
}
