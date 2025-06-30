using System;
using System.Collections.Generic;

namespace SaillingLoc.Models
{
    public class BoatType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Boat> Boats { get; set; } = new List<Boat>();
    }
}
