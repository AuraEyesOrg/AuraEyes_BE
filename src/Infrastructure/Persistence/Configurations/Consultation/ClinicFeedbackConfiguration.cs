using Domain.Entities.Consultation;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ClinicFeedbackConfiguration : IEntityTypeConfiguration<ClinicFeedback>
{
    public void Configure(EntityTypeBuilder<ClinicFeedback> builder)
    {
        builder.ToTable("ClinicFeedback", tableBuilder =>
            tableBuilder.HasCheckConstraint("CK_ClinicFeedback_Rating", "rating >= 1 AND rating <= 5"));

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Rating)
            .IsRequired();

        builder.Property(e => e.Comment)
            .HasMaxLength(1000);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        // Relationships
        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Appointment>()
            .WithMany()
            .HasForeignKey(e => e.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Ophthalmologist>()
            .WithMany()
            .HasForeignKey(e => e.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ClinicStaff>()
            .WithMany()
            .HasForeignKey(e => e.StaffId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.AppointmentId);
        builder.HasIndex(e => e.DoctorId);
        builder.HasIndex(e => e.StaffId);
    }
}
