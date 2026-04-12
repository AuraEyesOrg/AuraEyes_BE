using Domain.Entities.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        // String primary key — not a Guid
        builder.HasKey(e => e.Key);

        builder.Property(e => e.Key)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Value)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        // Seed default system settings
        builder.HasData(
            new { Key = "FREE_AI_QUOTA", Value = "3", Description = "Free AI screening credits per patient" },
            new { Key = "AI_QUOTA_UNIT_PRICE", Value = "10000", Description = "Price per 1 AI quota credit (VND)" },
            new { Key = "AI_QUOTA_PRICE", Value = "50000", Description = "Price per 5 additional AI credits (VND)" },
            new { Key = "AI_QUOTA_BUNDLE", Value = "5", Description = "Credits per paid bundle" },
            new { Key = "DEFAULT_PLATFORM_COMMISSION", Value = "0.20", Description = "Default platform commission rate (20%)" },
            new { Key = "MIN_ADVANCE_BOOKING_HOURS", Value = "0.5", Description = "Minimum time notice to book a slot (hours)" }
        );
    }
}
