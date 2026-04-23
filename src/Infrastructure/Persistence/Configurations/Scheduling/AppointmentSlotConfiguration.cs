using Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Scheduling;

public class AppointmentSlotConfiguration : IEntityTypeConfiguration<AppointmentSlot>
{
    public void Configure(EntityTypeBuilder<AppointmentSlot> builder)
    {
        builder.ToTable("AppointmentSlots", tableBuilder =>
        {
            // Database-level enforcement: booked_count <= capacity
            tableBuilder.HasCheckConstraint(
                "CK_AppointmentSlots_BookedCount_Capacity",
                "\"BookedCount\" <= \"MaxCapacity\"");
        });

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Source)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.SlotSource.Doctor);

        builder.Property(e => e.MaxCapacity)
            .HasDefaultValue(1);

        builder.Property(e => e.BookedCount)
            .HasDefaultValue(0);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(e => e.ScheduleTemplate)
            .WithMany(t => t.AppointmentSlots)
            .HasForeignKey(e => e.ScheduleTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Appointments)
            .WithOne(a => a.AppointmentSlot)
            .HasForeignKey(a => a.AppointmentSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.SlotAssignments)
            .WithOne(sa => sa.AppointmentSlot)
            .HasForeignKey(sa => sa.AppointmentSlotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.ScheduleTemplateId);
        builder.HasIndex(e => e.Date);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => new { e.ScheduleTemplateId, e.Date, e.StartTime, e.EndTime })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false")
            .HasDatabaseName("UX_AppointmentSlots_TemplateDateTime");
        builder.HasIndex(e => new { e.Status, e.BookedCount, e.MaxCapacity })
            .HasDatabaseName("IX_AppointmentSlots_Capacity");
    }
}
