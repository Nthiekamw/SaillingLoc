using System;

namespace SaillingLoc.Models
{
    public class Message
    {
        public int Id { get; set; }

        // Références aux utilisateurs
        public string SenderId { get; set; }     // ID de l'expéditeur
        public string ReceiverId { get; set; }   // ID du destinataire

        // Contenu et état du message
        public string Content { get; set; }
        public bool IsRead { get; set; } = false;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User Sender { get; set; }
        public User Receiver { get; set; }

        // Lié à une réservation
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
    }
}
