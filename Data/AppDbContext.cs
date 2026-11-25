using Microsoft.EntityFrameworkCore;
using PlatformWellSync.Models;

namespace PlatformWellSync.Data;

public class AppDbContext : DbContext
{
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<Well> Wells => Set<Well>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var connectionString = Environment.GetEnvironmentVariable("AEM_DB_CONNECTION")
        ?? "Server=localhost,1433;Database=AemEnersolDb;User Id=sa;Password=Aem@Test123;TrustServerCertificate=True;";

        options.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Platform>().ToTable("Platform");    
        modelBuilder.Entity<Well>().ToTable("Well");

        // Configure the one-to-many relationship between Platform and Well
        modelBuilder.Entity<Platform>()
            .HasMany(p => p.Wells)
            .WithOne(w => w.Platform)
            .HasForeignKey(w => w.PlatformId);
    }
}