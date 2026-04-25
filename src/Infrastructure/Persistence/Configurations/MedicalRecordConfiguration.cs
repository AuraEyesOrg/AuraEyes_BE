using Domain.Entities.MedicalRecords;
using Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for MedicalRecord entity.
/// </summary>
public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MedicalRecordNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.AdministrativeDataJson)
            .HasColumnType("text");

        builder.Property(x => x.ClinicalDataJson)
            .HasColumnType("text");

        builder.Property(x => x.PdfUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne<PatientVisit>()
            .WithOne(v => v.MedicalRecord)
            .HasForeignKey<MedicalRecord>(x => x.PatientVisitId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes for common queries
        builder.HasIndex(x => x.PatientId);
        builder.HasIndex(x => x.PatientVisitId);
        builder.HasIndex(x => x.MedicalRecordNumber).IsUnique();
        builder.HasIndex(x => x.IsDeleted);

        // Global query filter for soft delete
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
