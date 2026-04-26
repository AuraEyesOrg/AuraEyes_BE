using Domain.Entities.Consultation;
using Domain.Entities.Scheduling;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ConsultationSessionConfiguration : IEntityTypeConfiguration<ConsultationSession>
{
    public void Configure(EntityTypeBuilder<ConsultationSession> builder)
    {
        builder.ToTable("ConsultationSessions", table =>
        {
            table.HasCheckConstraint(
                "CK_ConsultationSessions_EndTime_After_StartTime",
                "\"StartTime\" IS NULL OR \"EndTime\" IS NULL OR \"EndTime\" >= \"StartTime\"");
        });

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.ChatStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Price)
            .HasPrecision(18, 2);

        builder.Property(e => e.IsRetinalImagesShared)
            .HasDefaultValue(false);

        builder.Property(e => e.IsAIResultShared)
            .HasDefaultValue(false);

        builder.Property(e => e.MeetingLink)
            .HasMaxLength(500);

        builder.Property(e => e.CalendarEventId)
            .HasMaxLength(1024);

        builder.Property(e => e.ClosingReason)
            .HasMaxLength(200);

        builder.Property(e => e.LastActivityAt)
            .IsRequired();

        builder.Property(e => e.LastReminderSentAt)
            .IsRequired(false);

        builder.Property(e => e.StartTime)
            .IsRequired(false);

        builder.Property(e => e.EndTime)
            .IsRequired(false);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Ophthalmologist>()
            .WithMany()
            .HasForeignKey(e => e.OphthalmologistId)
            .OnDelete(DeleteBehavior.Restrict);



        builder.HasOne<AiScreening>()
            .WithMany()
            .HasForeignKey(e => e.AiScreeningId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AppointmentSlot)
            .WithMany()
            .HasForeignKey(e => e.AppointmentSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for common query patterns
        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.OphthalmologistId);
        builder.HasIndex(e => e.AppointmentSlotId);
        builder.HasIndex(e => new { e.OphthalmologistId, e.Status, e.StartTime, e.EndTime })
            .HasDatabaseName("IX_ConsultationSessions_WorkloadLookup");
        builder.HasIndex(e => new { e.Status, e.ChatStatus, e.LastActivityAt, e.LastReminderSentAt })
            .HasDatabaseName("IX_ConsultationSessions_StaleSessionLookup");
    }
}
