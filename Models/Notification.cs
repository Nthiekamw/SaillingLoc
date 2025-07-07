
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaillingLoc.Models
{

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }

    // Ajoute cette ligne
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

}