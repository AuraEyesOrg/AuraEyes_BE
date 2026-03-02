using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.SlotType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Cost)
            .HasPrecision(18, 2);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne<Ophthalmologist>()
            .WithMany()
            .HasForeignKey(e => e.OphthalmologistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Organisation>()
            .WithMany()
            .HasForeignKey(e => e.OrganisationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
