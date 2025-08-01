using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SaillingLoc.Pages
{
    public class MessagesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MessagesModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Message> Messages { get; set; } = new();

        [BindProperty]
        public string ReceiverId { get; set; }

        [BindProperty]
        public string Content { get; set; }

        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();

        public async Task OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                throw new InvalidOperationException("Utilisateur non trouvé.");
            }

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            if (isAdmin)
            {
                Messages = await _context.Messages
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .OrderByDescending(m => m.SentAt)
                    .ToListAsync();
            }
            else
            {
                Messages = await _context.Messages
                    .Where(m => m.SenderId == currentUser.Id || m.ReceiverId == currentUser.Id)
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .OrderByDescending(m => m.SentAt)
                    .ToListAsync();

                // Marquer tous les messages non lus reçus comme lus
                var unreadMessages = Messages.Where(m => !m.IsRead && m.ReceiverId == currentUser.Id).ToList();
                if (unreadMessages.Any())
                {
                    foreach (var msg in unreadMessages)
                    {
                        msg.IsRead = true;
                        msg.UpdatedAt = DateTime.UtcNow;
                    }
                    await _context.SaveChangesAsync();
                }

                // Réinitialiser le compteur de messages non lus
                if (currentUser.UnreadMessagesCount > 0)
                {
                    currentUser.UnreadMessagesCount = 0;
                    _context.Users.Update(currentUser);
                    await _context.SaveChangesAsync();
                }
            }

            Users = await _context.Users
                .Where(u => u.Id != currentUser.Id)
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.FullName ?? u.UserName
                }).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var sender = await _userManager.GetUserAsync(User);
            if (sender == null)
            {
                throw new InvalidOperationException("Utilisateur non trouvé.");
            }

            var message = new Message
            {
                SenderId = sender.Id,
                ReceiverId = ReceiverId,
                Content = Content,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);

            var receiver = await _context.Users.FindAsync(ReceiverId);
            if (receiver != null)
            {
                receiver.UnreadMessagesCount += 1;
                _context.Users.Update(receiver);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Messages");
        }
    }
}
