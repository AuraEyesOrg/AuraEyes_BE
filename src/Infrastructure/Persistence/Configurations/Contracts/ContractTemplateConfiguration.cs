using Domain.Entities.Contracts;
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

        // Navigation to variable metadata — EF maps Variables → _variables via convention
        builder.HasMany(t => t.Variables)
            .WithOne()
            .HasForeignKey(v => v.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        // Use the backing field _variables for EF to populate when loading
        builder.Navigation(t => t.Variables)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

