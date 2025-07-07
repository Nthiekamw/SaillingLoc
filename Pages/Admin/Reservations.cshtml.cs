using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaillingLoc.Pages.Admin
{
    public class ReservationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReservationsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Reservation> Reservations { get; set; }

        public async Task OnGetAsync()
        {
            Reservations = await _context.Reservations
                .Include(r => r.User)
                .ToListAsync();
        }
    }
}
