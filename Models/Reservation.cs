using System;

namespace SaillingLoc.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public int BoatId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Boat Boat { get; set; }
        public User User { get; set; }
        public Review Review { get; set; }
        public Contract Contract { get; set; }
        public Payment Payment { get; set; }
    }
}
