using System;

namespace SaillingLoc.Models
{
    public class BoatEquipment
    {
        public int Id { get; set; }
        public int BoatId { get; set; }
        public string EquipmentName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Boat Boat { get; set; }
    }
}
