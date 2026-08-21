using Domain.Imports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ImportErrorConfiguration : IEntityTypeConfiguration<ImportError>
{
    public void Configure(EntityTypeBuilder<ImportError> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.lineNumber)
            .IsRequired();

        builder.Property(x => x.rawLine)
            .IsRequired();

        builder.Property(x => x.reason)
            .IsRequired();

        // Foreign key to ImportLog (using shadow property)
        builder.Property<int>("ImportLogId");

        builder.HasOne<ImportLog>()
            .WithMany(x => x.Errors)
            .HasForeignKey("ImportLogId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}