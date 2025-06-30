using System;

namespace SaillingLoc.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; }      // Doit être string si User.Id est string
        public string ReceiverId { get; set; }    // Même remarque ici
        public int ReservationId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User Sender { get; set; }
        public User Receiver { get; set; }
        public Reservation Reservation { get; set; }
    }
}

