using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AiScreeningConfiguration : IEntityTypeConfiguration<AiScreening>
{
    public void Configure(EntityTypeBuilder<AiScreening> builder)
    {
        builder.Property(e => e.ModelVersion)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        // JSONB field for raw AI output
        builder.Property(e => e.RawJsonOutput)
            .HasColumnType("jsonb");

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships - configure from parent side with navigation properties
        builder.HasMany(e => e.RetinalImages)
            .WithOne()
            .HasForeignKey(r => r.AiScreeningId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.ScreeningResults)
            .WithOne()
            .HasForeignKey(s => s.AiScreeningId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
