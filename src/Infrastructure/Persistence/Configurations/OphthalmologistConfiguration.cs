using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OphthalmologistConfiguration : IEntityTypeConfiguration<Ophthalmologist>
{
    public void Configure(EntityTypeBuilder<Ophthalmologist> builder)
    {
        builder.Property(e => e.Bio)
            .HasMaxLength(2000);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.YearsOfExperience)
            .HasDefaultValue(0);

        builder.Property(e => e.IsVerified)
            .HasDefaultValue(false);

        builder.Property(e => e.VerificationStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .HasDefaultValue(VerificationStatus.PendingVerification);

        builder.Property(e => e.LicenseUrl)
            .HasMaxLength(500);

        builder.Property(e => e.DegreeUrl)
            .HasMaxLength(500);

        builder.Property(e => e.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(e => e.UserId)
            .IsUnique();
    }
}
