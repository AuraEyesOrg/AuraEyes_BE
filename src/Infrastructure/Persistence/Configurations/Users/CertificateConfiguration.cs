using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.Property(e => e.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.IssuingAuthority)
            .HasMaxLength(200);

        builder.Property(e => e.CertificateUrl)
            .HasMaxLength(500);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne<Ophthalmologist>()
            .WithMany(o => o.Certificates)
            .HasForeignKey(e => e.OphthalmologistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
