using Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Scheduling;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_Appointments_ClinicVisit_DoctorId_Null",
                "\"Type\" <> 'ClinicVisit' OR \"DoctorId\" IS NULL");
        });

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

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
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Doctor)
            .WithMany()
            .HasForeignKey(e => e.DoctorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.AppointmentSlot)
            .WithMany(s => s.Appointments)
            .HasForeignKey(e => e.AppointmentSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ConsultationSession)
            .WithMany()
            .HasForeignKey(e => e.ConsultationSessionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.OrganisationId);
        builder.HasIndex(e => e.DoctorId);
        builder.HasIndex(e => e.AppointmentSlotId);
        builder.HasIndex(e => e.ConsultationSessionId);
        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.Status);

        // Unique constraint: one patient can only have one active appointment per slot
        builder.HasIndex(e => new { e.PatientId, e.AppointmentSlotId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false AND \"Status\" NOT IN ('Cancelled', 'NoShow')")
            .HasDatabaseName("IX_Appointments_Patient_Slot_Unique");

        // Index for querying by type and status
        builder.HasIndex(e => new { e.Type, e.Status })
            .HasDatabaseName("IX_Appointments_Type_Status");
    }
}
