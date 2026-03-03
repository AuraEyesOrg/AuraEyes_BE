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

        builder.Property(e => e.Content)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(e => e.IsRead)
            .HasDefaultValue(false);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);
    }
}
