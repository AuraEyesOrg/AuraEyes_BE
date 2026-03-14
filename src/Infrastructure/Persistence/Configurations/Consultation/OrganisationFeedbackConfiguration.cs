using Domain.Entities.Consultation;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrganisationFeedbackConfiguration : IEntityTypeConfiguration<OrganisationFeedback>
{
    public void Configure(EntityTypeBuilder<OrganisationFeedback> builder)
    {
        builder.ToTable("OrganisationFeedback", tableBuilder =>
            tableBuilder.HasCheckConstraint("CK_OrganisationFeedback_Rating", "rating >= 1 AND rating <= 5"));

        builder.Property(e => e.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(e => e.OrganisationId)
            .HasColumnName("organisation_id")
            .IsRequired();

        builder.Property(e => e.AppointmentId)
            .HasColumnName("appointment_id")
            .IsRequired();

        builder.Property(e => e.Rating)
            .HasColumnName("rating")
            .IsRequired();

        builder.Property(e => e.Comment)
            .HasColumnName("comment")
            .HasMaxLength(2000);

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(e => e.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(e => e.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        builder.Property(e => e.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.HasIndex(e => new { e.PatientId, e.AppointmentId })
            .IsUnique();

        builder.HasIndex(e => e.OrganisationId);
        builder.HasIndex(e => e.AppointmentId);
        builder.HasIndex(e => e.CreatedAt);

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Organisation>()
            .WithMany()
            .HasForeignKey(e => e.OrganisationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Appointment>()
            .WithMany()
            .HasForeignKey(e => e.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
