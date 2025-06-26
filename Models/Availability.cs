using System;

namespace SaillingLoc.Models
{
    public class Availability
    {
        public int Id { get; set; }
        public int BoatId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Boat Boat { get; set; }
    }
}
