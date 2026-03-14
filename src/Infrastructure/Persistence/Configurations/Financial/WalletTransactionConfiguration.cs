using Domain.Entities.Financial;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.Property(e => e.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(e => e.TransactionType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.ReferenceType)
            .HasMaxLength(50);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(e => new { e.ReferenceType, e.ReferenceId });

        // Relationships - Restrict delete for financial data
        builder.HasOne<Wallet>()
            .WithMany(w => w.Transactions)
            .HasForeignKey(e => e.WalletId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
