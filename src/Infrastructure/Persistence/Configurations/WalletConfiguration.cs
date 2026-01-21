using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.Property(e => e.Balance)
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // One wallet per user
        builder.HasIndex(e => e.UserId)
            .IsUnique();
    }
}
