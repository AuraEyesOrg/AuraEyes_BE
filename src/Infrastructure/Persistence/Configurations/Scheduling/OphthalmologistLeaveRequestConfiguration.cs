using Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Scheduling;

public class OphthalmologistLeaveRequestConfiguration : IEntityTypeConfiguration<OphthalmologistLeaveRequest>
{
    public void Configure(EntityTypeBuilder<OphthalmologistLeaveRequest> builder)
    {
        builder.ToTable("OphthalmologistLeaveRequests", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_OphthalmologistLeaveRequests_DateRange",
                "\"EndDate\" >= \"StartDate\"");
        });

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.AdminNote)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Ophthalmologist)
            .WithMany()
            .HasForeignKey(x => x.OphthalmologistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OphthalmologistId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.OphthalmologistId, x.StartDate, x.EndDate });
        builder.HasIndex(x => x.CreatedAt);
    }
}