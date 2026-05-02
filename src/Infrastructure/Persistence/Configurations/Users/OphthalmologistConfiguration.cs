using Domain.Entities.Users;
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

        builder.Property(e => e.EmploymentType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(OphthalmologistEmploymentType.FullTime);

        builder.Property(e => e.LicenseUrl)
            .HasMaxLength(500);

        builder.Property(e => e.DegreeUrl)
            .HasMaxLength(500);

        builder.Property(e => e.RatingAverage)
            .HasPrecision(4, 2)
            .HasDefaultValue(0m);

        builder.Property(e => e.RatingCount)
            .HasDefaultValue(0);

        builder.Property(e => e.ConsultationFee)
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(e => e.AvailableLeaveDays)
            .HasPrecision(10, 2)
            .HasDefaultValue(0m);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(e => e.UserId)
            .IsUnique();
    }
}
