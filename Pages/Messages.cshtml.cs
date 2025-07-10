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

        // OnGetAsync: Méthode appelée lors du chargement de la page pour récupérer les messages.
        public async Task OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                // Gérer l'erreur si l'utilisateur ne peut pas être trouvé
                throw new InvalidOperationException("Utilisateur non trouvé.");
            }

            // Vérifie si l'utilisateur est un administrateur
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            // Récupérer les messages selon le rôle de l'utilisateur
            if (isAdmin)
            {
                // Admin voit tous les messages
                Messages = await _context.Messages
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .OrderByDescending(m => m.SentAt)
                    .ToListAsync();
            }
            else
            {
                // Utilisateur voit ses messages reçus ou envoyés
                Messages = await _context.Messages
                    .Where(m => m.SenderId == currentUser.Id || m.ReceiverId == currentUser.Id)
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .OrderByDescending(m => m.SentAt)
                    .ToListAsync();
            }

            // Liste des utilisateurs pour sélection (envoi de message)
            Users = await _context.Users
                .Where(u => u.Id != currentUser.Id) // on exclut soi-même
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.FullName ?? u.UserName // Utilisation de FullName ou UserName si FullName est null
                }).ToListAsync();
        }

        // OnPostAsync: Méthode appelée lors de l'envoi d'un message
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Recharge la page avec les messages et la liste des utilisateurs
                await OnGetAsync();
                return Page();
            }

            var sender = await _userManager.GetUserAsync(User);
            if (sender == null)
            {
                // Gérer l'erreur si l'utilisateur n'est pas trouvé
                throw new InvalidOperationException("Utilisateur non trouvé.");
            }

            // Créer un nouveau message
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

            // Ajoute le message à la base de données
            _context.Messages.Add(message);

            // Incrémente le compteur de messages non lus du destinataire
            var receiver = await _context.Users.FindAsync(ReceiverId);
            if (receiver != null)
            {
                receiver.UnreadMessagesCount += 1;
                _context.Users.Update(receiver);
            }

            // Sauvegarder les changements dans la base de données
            await _context.SaveChangesAsync();

            // Rediriger vers la page des messages après l'envoi
            return RedirectToPage("/Messages");
        }
    }
}






// using Microsoft.AspNetCore.Mvc.RazorPages;
// using Microsoft.EntityFrameworkCore;
// using SaillingLoc.Data;
// using SaillingLoc.Models;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// namespace SaillingLoc.Pages
// {
//     public class MessagesModel : PageModel
//     {
//         private readonly ApplicationDbContext _context;

//         public MessagesModel(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//         public List<Message> Messages { get; set; } = new();

//         public async Task OnGetAsync()
//         {
//             Messages = await _context.Messages
//                 .Include(m => m.Sender)
//                 .Include(m => m.Receiver)
//                 .ToListAsync();
//         }
//     }
// }
