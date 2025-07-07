using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using System.Threading.Tasks;

namespace SaillingLoc.Pages
{
    public class AdminDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int UserCount { get; set; }
        public int ReservationCount { get; set; }
        public int MessageCount { get; set; }
        public int BoatCount { get; set; }

        public async Task OnGetAsync()
        {
            UserCount = await _context.Users.CountAsync();
            ReservationCount = await _context.Reservations.CountAsync();
            MessageCount = await _context.Messages.CountAsync();
            BoatCount = await _context.Boats.CountAsync();
        }
    }
}
