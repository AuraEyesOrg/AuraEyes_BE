using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MedicalDiagnosisConfiguration : IEntityTypeConfiguration<MedicalDiagnosis>
{
    public void Configure(EntityTypeBuilder<MedicalDiagnosis> builder)
    {
        builder.Property(e => e.DiagnosisCode)
            .HasMaxLength(50);

        builder.Property(e => e.CodingSystem)
            .HasMaxLength(50);

        builder.Property(e => e.ClinicalFindings)
            .HasColumnType("text");

        builder.Property(e => e.SeverityLevel)
            .HasMaxLength(50);

        builder.Property(e => e.ConfidenceLevel)
            .HasPrecision(5, 2);

        builder.Property(e => e.TreatmentPlan)
            .HasColumnType("text");

        builder.Property(e => e.Recommendations)
            .HasColumnType("text");

        builder.Property(e => e.LifestyleAdvice)
            .HasColumnType("text");

        builder.Property(e => e.Status)
            .HasMaxLength(50);

        builder.Property(e => e.IsUrgent)
            .HasDefaultValue(false);

        builder.Property(e => e.IsReferralNeeded)
            .HasDefaultValue(false);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // ConsultationSessionId is now required (non-nullable)
        builder.HasOne<ConsultationSession>()
            .WithMany(s => s.MedicalDiagnoses)
            .HasForeignKey(e => e.ConsultationSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AiScreening>()
            .WithMany()
            .HasForeignKey(e => e.AiScreeningId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
