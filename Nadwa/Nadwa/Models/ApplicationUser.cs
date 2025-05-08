using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Nadwa.Models;

public class ApplicationUser : IdentityUser {
    [Required] public bool IsDeleted { get; set; } = false;
    [Required] public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Event>? Events { get; set; } = new List<Event>();
}