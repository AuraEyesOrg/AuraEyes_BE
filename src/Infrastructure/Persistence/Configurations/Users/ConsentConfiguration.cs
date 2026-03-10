using Domain.Entities.Screening;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ConsentConfiguration : IEntityTypeConfiguration<Consent>
{
    public void Configure(EntityTypeBuilder<Consent> builder)
    {
        builder.Property(e => e.IsAgreed)
            .HasDefaultValue(false);

        builder.Property(e => e.Content)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // 1:1 with AiScreening
        builder.HasOne<AiScreening>()
            .WithOne(a => a.Consent)
            .HasForeignKey<Consent>(e => e.AiScreeningId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
