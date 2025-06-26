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

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public User User { get; set; }

        public UserDocument()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
