using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .UseIdentityColumn(1, 1);

        builder.Property(o => o.CustomerId)
            .IsRequired();

        builder.Property(o => o.OrderDate)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(o => o.Currency)
            .HasMaxLength(3)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasMaxLength(50)
            .IsRequired();

        // Index on CustomerId for query/join performance
        builder.HasIndex(o => o.CustomerId);
    }
}
