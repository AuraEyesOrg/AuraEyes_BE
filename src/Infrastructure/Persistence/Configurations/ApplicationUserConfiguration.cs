using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for ApplicationUser entity.
/// Configures additional properties not covered by Identity defaults.
/// </summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Address)
            .HasMaxLength(500);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(1000);

        builder.Property(u => u.CitizenId)
            .HasMaxLength(12);

        // Indexes for common queries
        builder.HasIndex(u => u.IsDeleted);
        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.OrganizationId);

        // Global query filter for soft delete
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
