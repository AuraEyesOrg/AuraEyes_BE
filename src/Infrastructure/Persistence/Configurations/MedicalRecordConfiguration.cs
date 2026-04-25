using Domain.Entities.MedicalRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

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

        builder.Property(x => x.Status)
            .HasConversion<int>();
    }
}
