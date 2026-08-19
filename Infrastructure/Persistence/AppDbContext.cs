using Domain.Customers;
using Domain.Imports;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<ImportLog> ImportLogs => Set<ImportLog>();
    public DbSet<ImportError> ImportErrors => Set<ImportError>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.LegacyCustomerId)
            .IsUnique();

        modelBuilder.Entity<Customer>()
            .Property(c => c.LegacyCustomerId).HasMaxLength(10);

        modelBuilder.Entity<Customer>()
            .Property(c => c.FullName).HasMaxLength(100);

        modelBuilder.Entity<Customer>()
            .Property(c => c.Email).HasMaxLength(150);

        modelBuilder.Entity<Customer>()
            .Property(c => c.Tier).HasMaxLength(2);
    }
}
