using Domain.Entities.Contracts;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Contracts;

public class ContractTemplateVariableConfiguration : IEntityTypeConfiguration<ContractTemplateVariable>
{
    public void Configure(EntityTypeBuilder<ContractTemplateVariable> builder)
    {
        builder.ToTable("ContractTemplateVariables");

        builder.Property(e => e.Key)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Label)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.VariableType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.DefaultValue)
            .HasMaxLength(500);

        // SelectOptions stored as JSON array string, e.g. '["Full-time","Part-time"]'
        builder.Property(e => e.SelectOptions)
            .HasColumnType("text");

        builder.Property(e => e.Unit)
            .HasMaxLength(50);

        builder.Property(e => e.IsRequired)
            .HasDefaultValue(false);

        builder.Property(e => e.SortOrder)
            .HasDefaultValue(0);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Unique: one key per template
        builder.HasIndex(e => new { e.TemplateId, e.Key })
            .IsUnique();
    }
}
