using System;

namespace SaillingLoc.Models
{
    public class UserDocument
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string FileUrl { get; set; }

        public bool IsVerified { get; set; }

        public string Message { get; set; }

        // ✅ Correction ici : UserId doit être de type string
        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // ✅ Navigation property correcte
        public User User { get; set; }

        public UserDocument()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
