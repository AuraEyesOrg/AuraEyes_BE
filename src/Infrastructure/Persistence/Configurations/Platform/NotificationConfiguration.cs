using Domain.Entities.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Message)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.IsRead)
            .HasDefaultValue(false);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Configure JSONB column type for PostgreSQL
        // Allows flexible metadata storage with efficient querying
        builder.Property(e => e.Payload)
            .HasColumnType("jsonb");

        // Index for efficient user notification queries
        builder.HasIndex(e => e.UserId);

        // Composite index for unread notifications per user
        builder.HasIndex(e => new { e.UserId, e.IsRead });

        // Index for filtering by notification type
        builder.HasIndex(e => e.Type);
    }
}
