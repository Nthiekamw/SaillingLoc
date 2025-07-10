using System;
using System.ComponentModel.DataAnnotations;

namespace SaillingLoc.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public int BoatId { get; set; }
        public string UserId { get; set; }  // Clé étrangère vers User qui a effectué la réservation
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }

        // Laisser la colonne Status sans être utilisée pour la validation
    public string? BoatOwnerId { get; set; }  // Clé étrangère vers User (propriétaire du bateau)    

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Boat Boat { get; set; }  // La réservation appartient à un bateau

        // Relation avec l'utilisateur qui a fait la réservation
        public User User { get; set; }

        public Review Review { get; set; }
        public Contract Contract { get; set; }
        public Payment Payment { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();

        // Propriétaire du bateau
        public User? BoatOwner { get; set; }  // Propriétaire du bateau
       

    }
}
