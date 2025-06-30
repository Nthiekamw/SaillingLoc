using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Threading.Tasks;

namespace SaillingLoc.Pages
{
    public class BoatDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public BoatDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Boat Boat { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Boat = await _context.Boats
                .Include(b => b.Photos)
                .Include(b => b.Port)
                .Include(b => b.BoatType)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (Boat == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
