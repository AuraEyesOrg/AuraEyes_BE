using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Users;

public class OphthalmologistEmploymentTypeChangeRequestConfiguration : IEntityTypeConfiguration<OphthalmologistEmploymentTypeChangeRequest>
{
    public void Configure(EntityTypeBuilder<OphthalmologistEmploymentTypeChangeRequest> builder)
    {
        builder.ToTable("OphthalmologistEmploymentTypeChangeRequests", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_OphthalmologistEmploymentTypeChangeRequests_TargetDifferent",
                "\"CurrentEmploymentType\" <> \"TargetEmploymentType\"");
        });

        builder.Property(x => x.CurrentEmploymentType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.TargetEmploymentType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

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
        builder.HasIndex(x => x.CreatedAt);

        builder.HasIndex(x => new { x.OphthalmologistId, x.Status })
            .HasDatabaseName("UX_OphthalmologistEmploymentTypeChangeRequests_Pending")
            .IsUnique()
            .HasFilter("\"Status\" = 'Pending'");
    }
}
