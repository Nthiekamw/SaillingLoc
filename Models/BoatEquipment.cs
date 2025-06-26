using System;

namespace SaillingLoc.Models
{
    public class BoatEquipment
    {
        public int Id { get; set; }
        public int BoatId { get; set; }
        public string EquipmentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Boat Boat { get; set; }
    }
}
