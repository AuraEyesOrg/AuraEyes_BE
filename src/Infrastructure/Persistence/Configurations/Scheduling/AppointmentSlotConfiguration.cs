using Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Scheduling;

public class AppointmentSlotConfiguration : IEntityTypeConfiguration<AppointmentSlot>
{
    public void Configure(EntityTypeBuilder<AppointmentSlot> builder)
    {
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.SlotType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Cost)
            .HasPrecision(18, 2);

        builder.Property(e => e.BookedCount)
            .HasDefaultValue(0);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(e => e.ScheduleTemplate)
            .WithMany(t => t.AppointmentSlots)
            .HasForeignKey(e => e.ScheduleTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.ScheduleTemplateId);
        builder.HasIndex(e => e.Date);
        builder.HasIndex(e => e.Status);
    }
}
