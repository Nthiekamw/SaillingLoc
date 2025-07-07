using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaillingLoc.Pages
{
    public class MessagesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MessagesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Message> Messages { get; set; } = new();

        public async Task OnGetAsync()
        {
            Messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();
        }
    }
}
