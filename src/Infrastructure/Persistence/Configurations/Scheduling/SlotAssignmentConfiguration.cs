using Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Scheduling;

public class SlotAssignmentConfiguration : IEntityTypeConfiguration<SlotAssignment>
{
    public void Configure(EntityTypeBuilder<SlotAssignment> builder)
    {
        builder.ToTable("SlotAssignments");

        builder.Property(e => e.Role)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(e => e.AppointmentSlot)
            .WithMany(s => s.SlotAssignments)
            .HasForeignKey(e => e.AppointmentSlotId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.AppointmentSlotId);
        builder.HasIndex(e => e.StaffId);

        // Unique: one staff member can have one role per slot
        builder.HasIndex(e => new { e.AppointmentSlotId, e.StaffId, e.Role })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false")
            .HasDatabaseName("UX_SlotAssignments_Slot_Staff_Role");
    }
}
