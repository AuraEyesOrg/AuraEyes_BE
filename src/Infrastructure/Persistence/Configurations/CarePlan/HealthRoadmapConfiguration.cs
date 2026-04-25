using Domain.Entities.CarePlan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.CarePlan;

public class HealthRoadmapConfiguration : IEntityTypeConfiguration<HealthRoadmap>
{
    public void Configure(EntityTypeBuilder<HealthRoadmap> builder)
    {
        builder.ToTable("HealthRoadmaps");

        builder.Property(e => e.IsDeleted).HasDefaultValue(false);

        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // One roadmap per patient.
        builder.HasIndex(e => e.PatientId).IsUnique();

        builder.HasMany(e => e.Steps)
            .WithOne(s => s.Roadmap!)
            .HasForeignKey(s => s.RoadmapId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
