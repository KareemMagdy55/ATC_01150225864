using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nadwa.Validators;

namespace Nadwa.Models;

public class Event {
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Column(TypeName = "Text")]
    public string Description { get; set; } 
    
    [Column(TypeName = "Text")]
    public string Tags { get; set; }

    public string Category { get; set; }


    public string Venue { get; set; }

    [Required(ErrorMessage = "Event date is required.")]
    [DateValidator]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }
    
    public virtual ICollection<ApplicationUser>? Attendees { get; set; } = new List<ApplicationUser>();
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Event other) return false;
        return Id == other.Id;
    }

    
}