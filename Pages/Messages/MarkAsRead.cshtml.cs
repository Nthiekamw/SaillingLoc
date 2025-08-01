







using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SaillingLoc.Data;
using System;
using System.Threading.Tasks;
using SaillingLoc.Models;

namespace SaillingLoc.Pages.Messages
{
    public class MarkAsReadModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MarkAsReadModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
                return NotFound();

            if (!message.IsRead)
            {
                message.IsRead = true;
                message.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Messages");
        }
    }
}




































// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using SaillingLoc.Data;
// using SaillingLoc.Models;
// using System;
// using System.Threading.Tasks;

// namespace SaillingLoc.Pages.Messages
// {
//     public class MarkAsReadModel : PageModel
//     {
//         private readonly ApplicationDbContext _context;

//         public MarkAsReadModel(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//        public async Task<IActionResult> OnGetAsync(int messageId)
// {
//     var message = await _context.Messages.FindAsync(messageId);
//     if (message == null)
//         return NotFound();

//     if (!message.IsRead)
//     {
//         message.IsRead = true;
//         message.UpdatedAt = DateTime.UtcNow;

//         // ✅ Récupérer le destinataire
//         var receiver = await _context.Users.FindAsync(message.ReceiverId);
//         if (receiver != null && receiver.UnreadMessagesCount > 0)
//         {
//             receiver.UnreadMessagesCount -= 1; // ✅ Décrémenter manuellement
//         }

//         await _context.SaveChangesAsync(); // ✅ Sauvegarder tous les changements
//     }

//     return RedirectToPage("/Messages");
// }

//     }
// }
