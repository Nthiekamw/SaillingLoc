using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaillingLoc.Data;
using SaillingLoc.Hubs;
using SaillingLoc.Models;

namespace SaillingLoc.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public NotificationService(
            ApplicationDbContext dbContext,
            IHubContext<NotificationHub> hubContext,
            UserManager<User> userManager,
            IEmailService emailService)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<int> GetUnreadNotificationsCountAsync(string userId)
        {
            return await _dbContext.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task CreateNotificationAsync(string userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                try
                {
                    await _emailService.SendEmailAsync(user.Email, "Nouvelle notification", message);
                }
                catch (Exception ex)
                {
                    // Loger ou gérer l'erreur d'email (optionnel)
                    Console.WriteLine($"Erreur envoi email: {ex.Message}");
                }
            }
        }

        public async Task NotifyAllUsersNewBoatAsync(string boatName)
        {
            var users = await _dbContext.Users.ToListAsync();

            // Ajout des notifications en base pour tous les utilisateurs
            foreach (var user in users)
            {
                var message = $"Nouvelle annonce de bateau disponible : {boatName}";
                var notif = new Notification
                {
                    UserId = user.Id,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.Notifications.Add(notif);
            }
            await _dbContext.SaveChangesAsync();

            // Envoi SignalR + Email pour chaque utilisateur
            foreach (var user in users)
            {
                var message = $"Nouvelle annonce de bateau disponible : {boatName}";

                await _hubContext.Clients.User(user.Id).SendAsync("ReceiveNotification", message);

                if (!string.IsNullOrEmpty(user.Email))
                {
                    try
                    {
                        await _emailService.SendEmailAsync(user.Email, "Nouvelle annonce bateau", message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur envoi email à {user.Email}: {ex.Message}");
                    }
                }
            }
        }
    }
}
