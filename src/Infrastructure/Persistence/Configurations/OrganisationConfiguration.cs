using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
{
    public void Configure(EntityTypeBuilder<Organisation> builder)
    {
        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.OrgType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.LicenseNumber)
            .HasMaxLength(100);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);
    }
}
