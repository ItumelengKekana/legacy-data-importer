using Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.LegacyCustomerId)
            .IsUnique();

        builder.Property(c => c.LegacyCustomerId)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(c => c.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Tier)
            .HasMaxLength(2)
            .IsRequired();
    }
}
