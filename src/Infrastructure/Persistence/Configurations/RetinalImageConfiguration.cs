using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RetinalImageConfiguration : IEntityTypeConfiguration<RetinalImage>
{
    public void Configure(EntityTypeBuilder<RetinalImage> builder)
    {
        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.EyeSide)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(e => e.DeviceName)
            .HasMaxLength(200);

        builder.Property(e => e.QualityScore)
            .HasPrecision(5, 2);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships are configured from parent configurations:
        // - PatientId: configured in PatientConfiguration
        // - AiScreeningId: configured in AiScreeningConfiguration
    }
}
