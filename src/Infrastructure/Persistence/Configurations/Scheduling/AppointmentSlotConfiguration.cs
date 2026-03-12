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

        builder.Property(e => e.MaxCapacity)
            .HasDefaultValue(1);

        builder.Property(e => e.BookedCount)
            .HasDefaultValue(0);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Reservation tracking
        builder.Property(e => e.ReservedBy)
            .IsRequired(false);

        builder.Property(e => e.ReservationExpireAt)
            .IsRequired(false);

        // Relationships
        builder.HasOne(e => e.ScheduleTemplate)
            .WithMany(t => t.AppointmentSlots)
            .HasForeignKey(e => e.ScheduleTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        // ClinicAppointments relationship
        builder.HasMany(e => e.ClinicAppointments)
            .WithOne(a => a.AppointmentSlot)
            .HasForeignKey(a => a.AppointmentSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.ScheduleTemplateId);
        builder.HasIndex(e => e.Date);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => new { e.Status, e.ReservationExpireAt })
            .HasFilter("\"Status\" = 'Reserved'");
        builder.HasIndex(e => new { e.Status, e.BookedCount, e.MaxCapacity })
            .HasDatabaseName("IX_AppointmentSlots_Capacity");
    }
}
