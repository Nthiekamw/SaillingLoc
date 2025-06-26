using System;

namespace SaillingLoc.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public int ReservationId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public decimal CommissionAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Reservation Reservation { get; set; }
    }
}
