using Domain.Entities.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.Property(e => e.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.EntityName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.EntityId)
            .HasMaxLength(100);

        // JSONB fields for old and new values
        builder.Property(e => e.OldValue)
            .HasColumnType("jsonb");

        builder.Property(e => e.NewValue)
            .HasColumnType("jsonb");

        builder.Property(e => e.IpAddress)
            .HasMaxLength(50);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);
    }
}
