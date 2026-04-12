using Domain.Entities.Financial;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Financial;

public class WithdrawalRequestConfiguration : IEntityTypeConfiguration<WithdrawalRequest>
{
    public void Configure(EntityTypeBuilder<WithdrawalRequest> builder)
    {
        builder.ToTable("WithdrawalRequests");

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.BankName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.BankAccountNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.AccountHolderName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ContractNumber)
            .HasMaxLength(100);

        builder.Property(x => x.Note)
            .HasMaxLength(1000);

        builder.Property(x => x.AdminNote)
            .HasMaxLength(1000);

        builder.Property(x => x.TransferReference)
            .HasMaxLength(200);

        builder.Property(x => x.BankBin)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(x => x.PayOSReferenceId)
            .HasMaxLength(100);

        builder.Property(x => x.ExternalPayoutId)
            .HasMaxLength(100);

        builder.Property(x => x.PayOSTransactionId)
            .HasMaxLength(100);

        builder.Property(x => x.PayOSApprovalState)
            .HasMaxLength(30);

        builder.Property(x => x.Fee)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);

        builder.HasOne(x => x.Wallet)
            .WithMany()
            .HasForeignKey(x => x.WalletId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
