using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        // JSONB field for medical history
        builder.Property(e => e.MedicalHistorySummary)
            .HasColumnType("jsonb");

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(e => e.UserId)
            .IsUnique();

        // Relationships - configure from parent side with navigation properties
        builder.HasMany(e => e.RetinalImages)
            .WithOne()
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Consents relationship is configured from ConsentConfiguration
    }
}
