using System;
using System.Collections.Generic;

namespace SaillingLoc.Models
{
    public class Port
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Boat> Boats { get; set; }
    }
}
