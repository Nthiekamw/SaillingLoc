using System;

namespace SaillingLoc.Models
{
    public class BoatPhoto
    {
        public int Id { get; set; }
        public int BoatId { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsMain { get; set; }
        public DateTime CreatedAt { get; set; }

        public Boat Boat { get; set; }
    }
}
