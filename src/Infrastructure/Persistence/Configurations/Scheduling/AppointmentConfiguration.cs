using Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Scheduling;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.VisitReason)
            .HasMaxLength(500);

        builder.Property(e => e.CancellationReason)
            .HasMaxLength(500);

        builder.Property(e => e.PricingType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Price)
            .HasPrecision(18, 2);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AppointmentSlot)
            .WithMany(s => s.Appointments)
            .HasForeignKey(e => e.AppointmentSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.RequestedDoctor)
            .WithMany()
            .HasForeignKey(e => e.RequestedDoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.AppointmentSlotId);
        builder.HasIndex(e => e.RequestedDoctorId);
        builder.HasIndex(e => e.Status);

        // Unique constraint: one active booking per patient/slot
        builder.HasIndex(e => new { e.PatientId, e.AppointmentSlotId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false AND \"Status\" NOT IN ('Cancelled', 'NoShow')")
            .HasDatabaseName("IX_Appointments_Patient_Slot_Unique");
    }
}
