using Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Scheduling;

public class ClinicAppointmentConfiguration : IEntityTypeConfiguration<ClinicAppointment>
{
    public void Configure(EntityTypeBuilder<ClinicAppointment> builder)
    {
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.VisitReason)
            .HasMaxLength(500);

        builder.Property(e => e.Notes)
            .HasMaxLength(2000);

        builder.Property(e => e.CancellationReason)
            .HasMaxLength(500);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Organisation)
            .WithMany()
            .HasForeignKey(e => e.OrganisationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AppointmentSlot)
            .WithMany(s => s.ClinicAppointments)
            .HasForeignKey(e => e.AppointmentSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AssignedDoctor)
            .WithMany()
            .HasForeignKey(e => e.AssignedDoctorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.OrganisationId);
        builder.HasIndex(e => e.AppointmentSlotId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => new { e.PatientId, e.AppointmentSlotId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false AND \"Status\" != 'Cancelled'")
            .HasDatabaseName("IX_ClinicAppointments_Patient_Slot_Unique");
    }
}
