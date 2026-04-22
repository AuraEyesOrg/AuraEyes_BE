using Domain.Entities.Platform;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WorkloadRequirementConfiguration : IEntityTypeConfiguration<WorkloadRequirement>
{
    public void Configure(EntityTypeBuilder<WorkloadRequirement> builder)
    {
        builder.ToTable("WorkloadRequirements", table =>
        {
            table.HasCheckConstraint(
                "CK_WorkloadRequirements_RequiredHours_NonNegative",
                "\"RequiredHours\" >= 0");
        });

        builder.Property(e => e.EmploymentType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.PeriodType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.RequiredHours)
            .HasPrecision(8, 2)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(e => new { e.EmploymentType, e.PeriodType })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false")
            .HasDatabaseName("UX_WorkloadRequirements_Employment_Period");

        builder.HasData(
            new
            {
                Id = Guid.Parse("4387bc41-31be-4d0e-a5a4-30bd95c79112"),
                EmploymentType = OphthalmologistEmploymentType.FullTime,
                PeriodType = WorkloadPeriodType.Week,
                RequiredHours = 40m,
                IsDeleted = false,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                UpdatedBy = "system"
            },
            new
            {
                Id = Guid.Parse("bc5004a0-a6ce-484a-a2e5-8af7559f95fb"),
                EmploymentType = OphthalmologistEmploymentType.FullTime,
                PeriodType = WorkloadPeriodType.Month,
                RequiredHours = 160m,
                IsDeleted = false,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                UpdatedBy = "system"
            });
    }
}
