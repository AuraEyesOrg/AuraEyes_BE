using Domain.Entities.Financial;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DepositRequestConfiguration : IEntityTypeConfiguration<DepositRequest>
{
    public void Configure(EntityTypeBuilder<DepositRequest> builder)
    {
        builder.Property(e => e.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(e => e.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.PaymentOrderCode)
            .HasMaxLength(100);

        builder.Property(e => e.PaymentUrl)
            .HasMaxLength(1000);

        builder.Property(e => e.ProviderTxnRef)
            .HasMaxLength(200);

        builder.Property(e => e.ProviderResponse)
            .HasMaxLength(4000);

        builder.Property(e => e.FailureReason)
            .HasMaxLength(500);

        builder.Property(e => e.ReturnUrl)
            .HasMaxLength(1000);

        builder.Property(e => e.CancelUrl)
            .HasMaxLength(1000);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Index for querying by order code
        builder.HasIndex(e => e.PaymentOrderCode);

        // Index for querying by user
        builder.HasIndex(e => e.UserId);

        // Relationships
        builder.HasOne(e => e.Wallet)
            .WithMany()
            .HasForeignKey(e => e.WalletId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
