using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.Property(e => e.BMI)
            .HasPrecision(5, 2);

        builder.Property(e => e.DiseaseHistory)
            .HasMaxLength(1000);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(e => e.UserId)
            .IsUnique();

        // Relationships
        builder.HasMany(e => e.RetinalImages)
            .WithOne()
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.AiScreenings)
            .WithOne()
            .HasForeignKey(s => s.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
