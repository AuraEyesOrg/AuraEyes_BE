using Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Scheduling;

public class DailySlotQuotaConfiguration : IEntityTypeConfiguration<DailySlotQuota>
{
    public void Configure(EntityTypeBuilder<DailySlotQuota> builder)
    {
        builder.HasKey(e => e.Date);

        builder.Property(e => e.Date)
            .HasColumnType("date");

        builder.Property(e => e.PartTimeSlotCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.QuotaSnapshot)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(e => e.UpdatedAt);
    }
}