using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Seasoned.Backend.Models;

namespace Seasoned.Backend.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Recipe> Recipes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        if (Database.IsNpgsql())
        {
            modelBuilder.HasPostgresExtension("vector");
        }

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(r => r.UserId);

            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                entity.Ignore(r => r.Embedding);
            }
            else
            {
                entity.Property(r => r.Embedding)
                    .HasColumnType("vector(768)"); 
            }
        });
    }
}