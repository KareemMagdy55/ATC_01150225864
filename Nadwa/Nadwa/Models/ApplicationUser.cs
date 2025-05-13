using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Nadwa.Models;

public class ApplicationUser : IdentityUser {
    
    [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [Required] public decimal Balance { get; set; } = 1000; 
    public virtual ICollection<Event>? Events { get; set; } = new List<Event>();
}