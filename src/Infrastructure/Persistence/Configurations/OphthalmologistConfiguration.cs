using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OphthalmologistConfiguration : IEntityTypeConfiguration<Ophthalmologist>
{
    public void Configure(EntityTypeBuilder<Ophthalmologist> builder)
    {
        builder.ToTable("Ophthalmologists");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.UserId)
            .IsRequired();

        builder.HasIndex(o => o.UserId)
            .IsUnique();

        builder.Property(o => o.Bio)
            .HasMaxLength(2000);

        builder.Property(o => o.YearsOfExperience)
            .IsRequired();

        builder.Property(o => o.IsVerified)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt);

        builder.Ignore(o => o.DomainEvents);
    }
}
