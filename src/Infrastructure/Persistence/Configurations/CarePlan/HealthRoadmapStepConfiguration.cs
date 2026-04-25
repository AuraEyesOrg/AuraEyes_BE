using Domain.Entities.CarePlan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.CarePlan;

public class HealthRoadmapStepConfiguration : IEntityTypeConfiguration<HealthRoadmapStep>
{
    public void Configure(EntityTypeBuilder<HealthRoadmapStep> builder)
    {
        builder.ToTable("HealthRoadmapSteps");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.StepType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.PlannedDate)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(e => e.Roadmap)
            .WithMany(r => r.Steps)
            .HasForeignKey(e => e.RoadmapId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.CreatedByDoctor)
            .WithMany()
            .HasForeignKey(e => e.CreatedByDoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CreatedFromVisit)
            .WithMany()
            .HasForeignKey(e => e.CreatedFromVisitId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => new { e.RoadmapId, e.PlannedDate });
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.CreatedFromVisitId);
    }
}
