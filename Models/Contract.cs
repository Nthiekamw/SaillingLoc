using System;

namespace SaillingLoc.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public int ReservationId { get; set; }
        public string ContractUrl { get; set; }
        public string OwnerSignature { get; set; }
        public string TenantSignature { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Reservation Reservation { get; set; }
    }
}
