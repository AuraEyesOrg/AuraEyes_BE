using Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Scheduling;

public class ScheduleTemplateConfiguration : IEntityTypeConfiguration<ScheduleTemplate>
{
    public void Configure(EntityTypeBuilder<ScheduleTemplate> builder)
    {
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_ScheduleTemplates_OphthalWithoutOrg",
            "\"OphthalId\" IS NULL OR \"OrgId\" IS NULL"));

        builder.Property(e => e.DayOfWeek)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.SlotDuration)
            .IsRequired();

        builder.Property(e => e.MaxCapacity)
            .IsRequired();

        builder.Property(e => e.Cost)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(e => e.OrgId)
            .IsRequired(false);

        builder.Property(e => e.OphthalId)
            .IsRequired(false);

        // Relationships
        builder.HasMany(e => e.AppointmentSlots)
            .WithOne(a => a.ScheduleTemplate)
            .HasForeignKey(a => a.ScheduleTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.OrgId);
        builder.HasIndex(e => e.OphthalId);
        builder.HasIndex(e => e.DayOfWeek);
    }
}
