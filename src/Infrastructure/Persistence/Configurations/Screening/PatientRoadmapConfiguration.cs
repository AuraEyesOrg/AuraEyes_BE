using Domain.Entities.Screening;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PatientRoadmapConfiguration : IEntityTypeConfiguration<PatientRoadmap>
{
    public void Configure(EntityTypeBuilder<PatientRoadmap> builder)
    {
        builder.ToTable("PatientRoadmaps");

        builder.Property(e => e.RiskLevel)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Summary)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.NextStepsJson)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(e => e.LifestyleAdviceJson)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(e => e.WarningSignsJson)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(e => e.FollowUpTimeframe)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(e => e.Source)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(e => e.RawAiResponse)
            .HasColumnType("text");

        builder.Property(e => e.GeneratedAt)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.MedicalDiagnosisId).IsUnique();
        builder.HasIndex(e => e.GeneratedAt);

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MedicalDiagnosis>()
            .WithMany()
            .HasForeignKey(e => e.MedicalDiagnosisId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
