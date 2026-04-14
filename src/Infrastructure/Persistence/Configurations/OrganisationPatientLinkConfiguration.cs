using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrganisationPatientLinkConfiguration : IEntityTypeConfiguration<OrganisationPatientLink>
{
    public void Configure(EntityTypeBuilder<OrganisationPatientLink> builder)
    {
        builder.Property(x => x.Source)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => new { x.OrganisationId, x.PatientId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(x => x.OrganisationId);
        builder.HasIndex(x => x.PatientId);
        builder.HasIndex(x => x.LastSeenAt);

        builder.HasOne(x => x.Organisation)
            .WithMany()
            .HasForeignKey(x => x.OrganisationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Patient)
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
