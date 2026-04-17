using Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Scheduling;

public class ExperiencePricingRuleConfiguration : IEntityTypeConfiguration<ExperiencePricingRule>
{
    public void Configure(EntityTypeBuilder<ExperiencePricingRule> builder)
    {
        builder.ToTable("ExperiencePricingRules", t =>
        {
            t.HasCheckConstraint(
                "CK_ExperiencePricingRules_YearsRange",
                "\"MinYearsExperience\" >= 0 AND \"MaxYearsExperience\" >= \"MinYearsExperience\"");

            t.HasCheckConstraint(
                "CK_ExperiencePricingRules_PriceRange",
                "\"MinPrice\" > 0 AND \"MaxPrice\" >= \"MinPrice\"");

            t.HasCheckConstraint(
                "CK_ExperiencePricingRules_IntegerPrice",
                "\"MinPrice\" = TRUNC(\"MinPrice\") AND \"MaxPrice\" = TRUNC(\"MaxPrice\")");
        });

        builder.Property(e => e.MinYearsExperience)
            .IsRequired();

        builder.Property(e => e.MaxYearsExperience)
            .IsRequired();

        builder.Property(e => e.MinPrice)
            .HasPrecision(18, 0)
            .IsRequired();

        builder.Property(e => e.MaxPrice)
            .HasPrecision(18, 0)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(e => new { e.MinYearsExperience, e.MaxYearsExperience })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false")
            .HasDatabaseName("UX_ExperiencePricingRules_YearsBand");

        builder.HasData(
            new
            {
                Id = Guid.Parse("65f0a3f8-17f0-4bc4-a97a-d5f11f63a2d1"),
                MinYearsExperience = 0,
                MaxYearsExperience = 2,
                MinPrice = 100000m,
                MaxPrice = 200000m,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                UpdatedBy = "system"
            },
            new
            {
                Id = Guid.Parse("1b7d3f39-e7b7-46e0-80ef-c5436a95f7af"),
                MinYearsExperience = 3,
                MaxYearsExperience = 5,
                MinPrice = 200000m,
                MaxPrice = 300000m,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                UpdatedBy = "system"
            },
            new
            {
                Id = Guid.Parse("70eb8d16-3709-4d06-a3d2-8f65f5f31184"),
                MinYearsExperience = 6,
                MaxYearsExperience = 70,
                MinPrice = 300000m,
                MaxPrice = 400000m,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                UpdatedBy = "system"
            });
    }
}