using Domain.Entities.Contracts;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ContractTemplateConfiguration : IEntityTypeConfiguration<ContractTemplate>
{
    public void Configure(EntityTypeBuilder<ContractTemplate> builder)
    {
        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.EmploymentType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(e => e.ContractVersion)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.ContentTemplate)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(e => new { e.Type, e.ContractVersion })
            .IsUnique();
    }
}

