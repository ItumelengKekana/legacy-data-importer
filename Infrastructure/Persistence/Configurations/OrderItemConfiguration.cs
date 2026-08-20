using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Id)
            .UseIdentityColumn(1, 1);

        builder.Property(oi => oi.Description)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(oi => oi.Sku)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(10, 2)")
            .IsRequired();

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        // Foreign Key relationship setup
        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign Key Index for JOIN performance
        builder.HasIndex(oi => oi.OrderId);
    }
}
