using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for RefreshToken entity.
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TokenHash)
            .IsRequired()
            .HasMaxLength(128); // SHA256 base64 encoded

        builder.Property(t => t.JwtId)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(t => t.DeviceInfo)
            .HasMaxLength(500);

        builder.Property(t => t.IpAddress)
            .HasMaxLength(45); // IPv6 max length

        builder.Property(t => t.RevokedReason)
            .HasMaxLength(100);

        // Index on TokenHash for fast lookup
        builder.HasIndex(t => t.TokenHash);

        // Index on UserId for user's tokens lookup
        builder.HasIndex(t => t.UserId);

        // Composite index for cleanup queries
        builder.HasIndex(t => new { t.ExpiresAt, t.RevokedAt });

        // Relationship with ApplicationUser
        builder.HasOne(t => t.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore computed properties
        builder.Ignore(t => t.IsExpired);
        builder.Ignore(t => t.IsRevoked);
        builder.Ignore(t => t.IsActive);

        // Query filter to match ApplicationUser's soft delete filter
        // This ensures tokens of soft-deleted users are also filtered out
        builder.HasQueryFilter(t => !t.User.IsDeleted);
    }
}
