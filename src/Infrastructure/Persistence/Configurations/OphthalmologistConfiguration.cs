using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OphthalmologistConfiguration : IEntityTypeConfiguration<Ophthalmologist>
{
    public void Configure(EntityTypeBuilder<Ophthalmologist> builder)
    {
        builder.Property(e => e.Bio)
            .HasMaxLength(2000);

        builder.Property(e => e.YearsOfExperience)
            .HasDefaultValue(0);

        builder.Property(e => e.IsVerified)
            .HasDefaultValue(false);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(e => e.UserId)
            .IsUnique();
    }
}
