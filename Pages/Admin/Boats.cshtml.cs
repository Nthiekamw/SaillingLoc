using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Collections.Generic;
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

        public List<Boat> Boats { get; set; }

        public async Task OnGetAsync()
        {
            Boats = await _context.Boats
                .Include(b => b.Port)
                .Include(b => b.BoatType)
                .ToListAsync();
        }
    }
}
