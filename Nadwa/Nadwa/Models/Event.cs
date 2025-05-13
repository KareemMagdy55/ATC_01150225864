using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nadwa.Models;

public class Event {
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Column(TypeName = "Text")]
    public string Description { get; set; }

    public string Category { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public string Venue { get; set; }

    public decimal Price { get; set; }

    public string ImageUrl { get; set; }
    
    public virtual ICollection<ApplicationUser>? Attendees { get; set; } = new List<ApplicationUser>();
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }

    
}