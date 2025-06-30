using System;

namespace SaillingLoc.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public int BoatId { get; set; }

        public string UserId { get; set; } // Doit correspondre à la clé primaire de User, donc string

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Boat Boat { get; set; }

        // Relation avec User
        public User User { get; set; }

        public Review Review { get; set; }
        public Contract Contract { get; set; }
        public Payment Payment { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();

    }
}
