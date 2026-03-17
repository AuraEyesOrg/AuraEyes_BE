using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrganisationOnboardingRequestConfiguration : IEntityTypeConfiguration<OrganisationOnboardingRequest>
{
    public void Configure(EntityTypeBuilder<OrganisationOnboardingRequest> builder)
    {
        builder.Property(e => e.OrganisationName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.ContactFullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.ContactEmail)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.ContactPhone)
            .HasMaxLength(50);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.LicenseNumber)
            .HasMaxLength(100);

        builder.Property(e => e.Notes)
            .HasColumnType("text");

        builder.Property(e => e.OrgType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(e => new { e.ContactEmail, e.Status });
    }
}