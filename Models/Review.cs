using System;

namespace SaillingLoc.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Response { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Reservation Reservation { get; set; }
    }
}
