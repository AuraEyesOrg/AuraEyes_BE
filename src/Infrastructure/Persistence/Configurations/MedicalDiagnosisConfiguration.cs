using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MedicalDiagnosisConfiguration : IEntityTypeConfiguration<MedicalDiagnosis>
{
    public void Configure(EntityTypeBuilder<MedicalDiagnosis> builder)
    {
        builder.Property(e => e.DiagnosesCode)
            .HasMaxLength(50);

        builder.Property(e => e.DiagnosesText)
            .HasMaxLength(2000);

        builder.Property(e => e.TreatmentPlan)
            .HasMaxLength(2000);

        builder.Property(e => e.ReferralRequired)
            .HasDefaultValue(false);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships - Restrict delete for medical data
        builder.HasOne<AiScreening>()
            .WithMany()
            .HasForeignKey(e => e.AiScreeningId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ConsultationSession>()
            .WithMany(s => s.MedicalDiagnoses)
            .HasForeignKey(e => e.ConsultationSessionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
