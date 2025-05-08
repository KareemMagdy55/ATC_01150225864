using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nadwa.Models;

namespace Nadwa.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);
        
        builder.Entity<ApplicationUser>()
            .Navigation(u => u.Events)
            .EnableLazyLoading();
        
        builder.Entity<Event>()
            .Navigation(e => e.Attendees)
            .EnableLazyLoading();
        
    }
}