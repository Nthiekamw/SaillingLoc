using System.Threading.Tasks;
using System;
using System.Collections.Generic;
// Autres using nécessaires


namespace SaillingLoc.Services
{
    public interface INotificationService
    {
        Task<int> GetUnreadNotificationsCountAsync(string userId);

        /// <summary>
        /// Crée une notification pour un utilisateur spécifique, envoie SignalR et email.
        /// </summary>
        /// <param name="userId">L'ID de l'utilisateur</param>
        /// <param name="message">Le message de la notification</param>
        Task CreateNotificationAsync(string userId, string message);

        /// <summary>
        /// Notifie tous les utilisateurs d'un nouveau bateau disponible.
        /// </summary>
        /// <param name="boatName">Le nom du bateau</param>
        Task NotifyAllUsersNewBoatAsync(string boatName);
    }
}
