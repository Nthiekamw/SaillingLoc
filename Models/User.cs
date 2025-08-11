using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace SaillingLoc.Models
{
    public class User : IdentityUser
    {
        public string? Reference { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string? PaymentMethod { get; set; }     

        public string? Photo { get; set; }
        public string? Address { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string FullName => $"{FirstName} {LastName}";
         

    // Ajoute un champ pour le nombre de messages non lus
    public int UnreadMessagesCount { get; set; } = 0;

  


        // Navigation properties
        public ICollection<UserDocument> Documents { get; set; }
        public ICollection<Boat> Boats { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
