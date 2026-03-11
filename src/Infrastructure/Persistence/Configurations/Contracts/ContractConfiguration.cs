using Domain.Entities.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.Property(e => e.ContractNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.ScannedDocumentUrl)
            .HasMaxLength(500);

        builder.Property(e => e.AiQuotaLimit)
            .HasDefaultValue(0);

        builder.Property(e => e.PlatformCommissionRate)
            .HasPrecision(5, 4)
            .HasDefaultValue(0m);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships - Restrict delete for financial data
        builder.HasOne<ContractTemplate>()
            .WithMany()
            .HasForeignKey(e => e.TemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.ContractNumber)
            .IsUnique();
    }
}
