using Domain.Entities.Consultation;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OphthalmologistFeedbackConfiguration : IEntityTypeConfiguration<OphthalmologistFeedback>
{
    public void Configure(EntityTypeBuilder<OphthalmologistFeedback> builder)
    {
        builder.ToTable("OphthalmologistFeedback", tableBuilder =>
            tableBuilder.HasCheckConstraint("CK_OphthalmologistFeedback_Rating", "rating >= 1 AND rating <= 5"));

        builder.Property(e => e.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(e => e.OphthalmologistId)
            .HasColumnName("ophthalmologist_id")
            .IsRequired();

        builder.Property(e => e.ConsultationSessionId)
            .HasColumnName("consultation_session_id")
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

        builder.HasIndex(e => new { e.PatientId, e.ConsultationSessionId })
            .IsUnique();

        builder.HasIndex(e => e.OphthalmologistId);
        builder.HasIndex(e => e.ConsultationSessionId);
        builder.HasIndex(e => e.CreatedAt);

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Ophthalmologist>()
            .WithMany()
            .HasForeignKey(e => e.OphthalmologistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ConsultationSession>()
            .WithMany()
            .HasForeignKey(e => e.ConsultationSessionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
