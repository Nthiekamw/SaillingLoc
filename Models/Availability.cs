



using System;

namespace SaillingLoc.Models
{
    public class Availability
    {
        public int Id { get; set; }
        public int BoatId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Propriété de navigation vers le bateau
        public Boat Boat { get; set; }

        // Constructeur pour initialiser les dates de création et de mise à jour
        public Availability()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // Méthode pour vérifier que les dates sont valides (startDate < endDate et startDate >= DateTime.UtcNow)
        public bool AreDatesValid()
        {
            return StartDate < EndDate && StartDate >= DateTime.UtcNow;
        }
    }
}


























// using System;

// namespace SaillingLoc.Models
// {
//     public class Availability
//     {
//         public int Id { get; set; }
//         public int BoatId { get; set; }
//         public DateTime StartDate { get; set; }
//         public DateTime EndDate { get; set; }
//         public string Status { get; set; }
//         public DateTime CreatedAt { get; set; }
//         public DateTime UpdatedAt { get; set; }

//         public Boat Boat { get; set; }

//         public Availability()
//         {
//             CreatedAt = DateTime.UtcNow;
//             UpdatedAt = DateTime.UtcNow;
//         }
//     }
// }
