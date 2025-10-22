using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaillingLoc.Pages
{
    public class ConversationViewModel
    {
        public string ConversationId { get; set; }
        public string ConversationTitle { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserId { get; set; }
        public DateTime LastMessageDate { get; set; }
        public int UnreadCount { get; set; }
        public string LastMessagePreview { get; set; }
    }

    public class MessagesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MessagesModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<ConversationViewModel> Conversations { get; set; } = new();
        public List<Message> CurrentMessages { get; set; } = new();
        public List<User> AllUsers { get; set; } = new(); // Liste de tous les utilisateurs pour le formulaire
        public string CurrentUserId { get; set; }
        public string SelectedConversationId { get; set; }
        public string CurrentConversationTitle { get; set; }
        public string CurrentConversationSubtitle { get; set; }
        public string CurrentReceiverId { get; set; }

        [BindProperty]
        public string ConversationId { get; set; }

        [BindProperty]
        public string ReceiverId { get; set; }

        [BindProperty]
        public string MessageContent { get; set; }

        public async Task<IActionResult> OnGetAsync(string conversationId = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Redirect("/Identity/Account/Login?returnUrl=/Messages");
            }

            CurrentUserId = currentUser.Id;
            SelectedConversationId = conversationId;

            // Charger la liste de tous les utilisateurs (sauf l'utilisateur actuel)
            AllUsers = await _context.Users
                .Where(u => u.Id != currentUser.Id)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();

            // Récupérer tous les messages de l'utilisateur
            var allMessages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == currentUser.Id || m.ReceiverId == currentUser.Id)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            // Grouper les messages par conversation (paires d'utilisateurs)
            var conversationGroups = allMessages
                .GroupBy(m => m.SenderId == currentUser.Id ? m.ReceiverId : m.SenderId)
                .Select(g => new
                {
                    OtherUserId = g.Key,
                    Messages = g.OrderByDescending(m => m.SentAt).ToList(),
                    LastMessage = g.OrderByDescending(m => m.SentAt).First(),
                    UnreadCount = g.Count(m => !m.IsRead && m.ReceiverId == currentUser.Id)
                })
                .OrderByDescending(c => c.LastMessage.SentAt)
                .ToList();

            // Créer les ViewModels pour les conversations
            Conversations = new List<ConversationViewModel>();
            foreach (var group in conversationGroups)
            {
                var otherUser = await _context.Users.FindAsync(group.OtherUserId);
                if (otherUser != null)
                {
                    // Identifier la conversation par le bateau concerné (si applicable)
                    var conversationTitle = "Sea Ray 240"; // Exemple - à adapter selon votre logique
                    
                    // Vous pouvez récupérer le bateau depuis le contexte du premier message
                    // Par exemple, si les messages contiennent une référence au bateau
                    
                    Conversations.Add(new ConversationViewModel
                    {
                        ConversationId = group.OtherUserId,
                        ConversationTitle = conversationTitle,
                        OtherUserName = $"Avec {otherUser.FullName}",
                        OtherUserId = group.OtherUserId,
                        LastMessageDate = group.LastMessage.SentAt,
                        UnreadCount = group.UnreadCount,
                        LastMessagePreview = group.LastMessage.Content?.Length > 50 
                            ? group.LastMessage.Content.Substring(0, 50) + "..." 
                            : group.LastMessage.Content
                    });
                }
            }

            // Si une conversation est sélectionnée, charger ses messages
            if (!string.IsNullOrEmpty(conversationId))
            {
                CurrentReceiverId = conversationId;
                
                CurrentMessages = allMessages
                    .Where(m => 
                        (m.SenderId == currentUser.Id && m.ReceiverId == conversationId) ||
                        (m.SenderId == conversationId && m.ReceiverId == currentUser.Id))
                    .OrderBy(m => m.SentAt)
                    .ToList();

                var otherUser = await _context.Users.FindAsync(conversationId);
                if (otherUser != null)
                {
                    CurrentConversationTitle = "Sea Ray 240"; // À adapter
                    CurrentConversationSubtitle = otherUser.FullName;
                }

                // Marquer les messages non lus comme lus
                var unreadMessages = CurrentMessages
                    .Where(m => !m.IsRead && m.ReceiverId == currentUser.Id)
                    .ToList();

                if (unreadMessages.Any())
                {
                    foreach (var msg in unreadMessages)
                    {
                        msg.IsRead = true;
                        msg.UpdatedAt = DateTime.UtcNow;
                    }
                    await _context.SaveChangesAsync();

                    // Mettre à jour le compteur
                    currentUser.UnreadMessagesCount = Math.Max(0, currentUser.UnreadMessagesCount - unreadMessages.Count);
                    _context.Users.Update(currentUser);
                    await _context.SaveChangesAsync();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(MessageContent))
            {
                return await OnGetAsync(ConversationId);
            }

            var sender = await _userManager.GetUserAsync(User);
            if (sender == null)
            {
                return Redirect("/Identity/Account/Login?returnUrl=/Messages");
            }

            // Créer le message
            var message = new Message
            {
                SenderId = sender.Id,
                ReceiverId = ReceiverId,
                Content = MessageContent,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);

            // Incrémenter le compteur de messages non lus pour le destinataire
            var receiver = await _context.Users.FindAsync(ReceiverId);
            if (receiver != null)
            {
                receiver.UnreadMessagesCount += 1;
                _context.Users.Update(receiver);
            }

            await _context.SaveChangesAsync();

            // Rediriger vers la même conversation
            return RedirectToPage("/Messages", new { conversationId = ReceiverId });
        }

        // Handler pour le nouveau message (depuis le modal)
        public async Task<IActionResult> OnPostNewMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(MessageContent) || string.IsNullOrWhiteSpace(ReceiverId))
            {
                return await OnGetAsync();
            }

            var sender = await _userManager.GetUserAsync(User);
            if (sender == null)
            {
                return Redirect("/Identity/Account/Login?returnUrl=/Messages");
            }

            // Créer le message
            var message = new Message
            {
                SenderId = sender.Id,
                ReceiverId = ReceiverId,
                Content = MessageContent,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);

            // Incrémenter le compteur de messages non lus pour le destinataire
            var receiver = await _context.Users.FindAsync(ReceiverId);
            if (receiver != null)
            {
                receiver.UnreadMessagesCount += 1;
                _context.Users.Update(receiver);
            }

            await _context.SaveChangesAsync();

            // Rediriger vers la nouvelle conversation
            return RedirectToPage("/Messages", new { conversationId = ReceiverId });
        }
    }
}



















// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using Microsoft.AspNetCore.Identity;
// using SaillingLoc.Data;
// using SaillingLoc.Models;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using System;

// namespace SaillingLoc.Pages
// {
//     public class MessagesModel : PageModel
//     {
//         private readonly ApplicationDbContext _context;
//         private readonly UserManager<User> _userManager;

//         public MessagesModel(ApplicationDbContext context, UserManager<User> userManager)
//         {
//             _context = context;
//             _userManager = userManager;
//         }

//         public List<Message> Messages { get; set; } = new();

//         [BindProperty]
//         public string ReceiverId { get; set; }

//         [BindProperty]
//         // public string Content { get; set; }
       
// public string MessageContent { get; set; }


//         public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();

//         public async Task OnGetAsync()
//         {
//             var currentUser = await _userManager.GetUserAsync(User);
//             if (currentUser == null)
// {
//     // Rediriger vers la page de connexion (avec retour ici après connexion)
//     Response.Redirect("/Identity/Account/Login?returnUrl=/Messages");
//     return;
// }

//             // if (currentUser == null)
//             // {
//             //     throw new InvalidOperationException("Utilisateur non trouvé.");
//             // }

//             var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

//             if (isAdmin)
//             {
//                 Messages = await _context.Messages
//                     .Include(m => m.Sender)
//                     .Include(m => m.Receiver)
//                     .OrderByDescending(m => m.SentAt)
//                     .ToListAsync();
//             }
//             else
//             {
//                 Messages = await _context.Messages
//                     .Where(m => m.SenderId == currentUser.Id || m.ReceiverId == currentUser.Id)
//                     .Include(m => m.Sender)
//                     .Include(m => m.Receiver)
//                     .OrderByDescending(m => m.SentAt)
//                     .ToListAsync();

//                 // Marquer tous les messages non lus reçus comme lus
//                 var unreadMessages = Messages.Where(m => !m.IsRead && m.ReceiverId == currentUser.Id).ToList();
//                 if (unreadMessages.Any())
//                 {
//                     foreach (var msg in unreadMessages)
//                     {
//                         msg.IsRead = true;
//                         msg.UpdatedAt = DateTime.UtcNow;
//                     }
//                     await _context.SaveChangesAsync();
//                 }

//                 // Réinitialiser le compteur de messages non lus
//                 if (currentUser.UnreadMessagesCount > 0)
//                 {
//                     currentUser.UnreadMessagesCount = 0;
//                     _context.Users.Update(currentUser);
//                     await _context.SaveChangesAsync();
//                 }
//             }

//             Users = await _context.Users
//                 .Where(u => u.Id != currentUser.Id)
//                 .Select(u => new SelectListItem
//                 {
//                     Value = u.Id,
//                     Text = u.FullName ?? u.UserName
//                 }).ToListAsync();
//         }

//         public async Task<IActionResult> OnPostAsync()
//         {
//             if (!ModelState.IsValid)
//             {
//                 await OnGetAsync();     
//                 return Page();
//             }

//             var sender = await _userManager.GetUserAsync(User);
//             if (sender == null)
//             {
//                 throw new InvalidOperationException("Utilisateur non trouvé.");
//             }

//             var message = new Message
//             {
//                 SenderId = sender.Id,
//                 ReceiverId = ReceiverId,
//                 Content = MessageContent,
//                 SentAt = DateTime.UtcNow,
//                 IsRead = false,
//                 CreatedAt = DateTime.UtcNow,
//                 UpdatedAt = DateTime.UtcNow
//             };

//             _context.Messages.Add(message);

//             var receiver = await _context.Users.FindAsync(ReceiverId);
//             if (receiver != null)
//             {
//                 receiver.UnreadMessagesCount += 1;
//                 _context.Users.Update(receiver);
//             }

//             await _context.SaveChangesAsync();

//             return RedirectToPage("/Messages");
//         }
//     }
// }
