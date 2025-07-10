using System;

namespace SaillingLoc.Models
{
    public class UserActionLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public string PageUrl { get; set; }
        public DateTime Timestamp { get; set; }

        public string IPAddress { get; set; }

        public virtual User User { get; set; }
    }
}
