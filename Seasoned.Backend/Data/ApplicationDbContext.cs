using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Seasoned.Backend.Models;

namespace Seasoned.Backend.Data;

// Inherit from IdentityDbContext to enable User management
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Recipe> Recipes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Crucial: Call the base method so Identity tables are configured
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("vector");

        // Optional: Ensure the Recipe table links to the Identity User
        modelBuilder.Entity<Recipe>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(r => r.UserId);
    }
}