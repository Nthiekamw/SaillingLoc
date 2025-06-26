using System;

namespace SaillingLoc.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int ReservationId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User Sender { get; set; }
        public User Receiver { get; set; }
        public Reservation Reservation { get; set; }
    }
}
