using Domain.Entities.Screening;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ScreeningResultConfiguration : IEntityTypeConfiguration<ScreeningResult>
{
    public void Configure(EntityTypeBuilder<ScreeningResult> builder)
    {
        builder.Property(e => e.RiskLevel)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.ConfidenceScore)
            .HasPrecision(5, 2);

        builder.Property(e => e.Summary)
            .HasMaxLength(1000);

        builder.Property(e => e.Findings)
            .HasMaxLength(2000);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // AiScreening relationship is configured from AiScreeningConfiguration
    }
}
