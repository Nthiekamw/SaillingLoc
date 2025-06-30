using System;
using System.ComponentModel.DataAnnotations;

namespace SaillingLoc.Models
{
    public class BoatPhoto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "L'URL de la photo est obligatoire")]
        public string PhotoUrl { get; set; } = null!;

        [Required]
        public int BoatId { get; set; }

        public bool IsMain { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property vers le bateau
        public Boat Boat { get; set; } = null!;
    }
}
