using Domain.Entities.Screening;
using Domain.Entities.Users;
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

        // PatientId FK — configured from Patient side (PatientConfiguration)


        // 1:1 Consent configured from ConsentConfiguration
        builder.HasMany(e => e.RetinalImages)
            .WithOne()
            .HasForeignKey(r => r.AiScreeningId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.ScreeningResults)
            .WithOne()
            .HasForeignKey(s => s.AiScreeningId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.PatientId);

    }
}
