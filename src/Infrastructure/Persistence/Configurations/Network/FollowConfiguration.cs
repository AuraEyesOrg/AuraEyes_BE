using Domain.Entities.Network;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Network;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ToTable("Follows");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FollowerId)
            .IsRequired();

        builder.Property(e => e.FollowerType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.FollowingId)
            .IsRequired();

        builder.Property(e => e.FollowingType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.NotificationsEnabled)
            .HasDefaultValue(true);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Unique constraint: one follower can only follow a target once
        builder.HasIndex(e => new { e.FollowerId, e.FollowingId })
            .IsUnique();

        // Indexes for performance
        builder.HasIndex(e => e.FollowerId);
        builder.HasIndex(e => e.FollowingId);
        builder.HasIndex(e => new { e.FollowerId, e.FollowerType });
        builder.HasIndex(e => new { e.FollowingId, e.FollowingType });
    }
}
