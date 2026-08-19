using Domain.Imports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ImportLogConfiguration : IEntityTypeConfiguration<ImportLog>
{
    public void Configure(EntityTypeBuilder<ImportLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.BatchId).IsUnique();

        // Encapsulated backing field for Errors navigation
        builder.HasMany(x => x.Errors)
            .WithOne()
            .HasForeignKey("ImportLogId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Errors)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
