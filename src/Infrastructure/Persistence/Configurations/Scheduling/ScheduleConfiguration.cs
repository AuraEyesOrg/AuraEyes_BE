using Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.SlotType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Cost)
            .HasPrecision(18, 2);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships - FK to AvailableSlot (replaces direct doctor/org FKs, satisfies 3NF)
        builder.HasOne(e => e.AvailableSlot)
            .WithMany(a => a.Schedules)
            .HasForeignKey(e => e.AvailableSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.AvailableSlotId);
        builder.HasIndex(e => e.PatientId);
    }
}
