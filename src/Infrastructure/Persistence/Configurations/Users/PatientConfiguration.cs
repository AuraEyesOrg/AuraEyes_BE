using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        // UserId is nullable (walk-in patients have no Identity user)
        builder.Property(e => e.UserId)
            .IsRequired(false);

        // ── Walk-in profile fields ──

        builder.Property(e => e.FullName)
            .HasMaxLength(200);

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(e => e.CitizenId)
            .HasMaxLength(20);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        // ── EMR fields ──

        builder.Property(e => e.BMI)
            .HasPrecision(5, 2);

        builder.Property(e => e.DiseaseHistory)
            .HasMaxLength(1000);

        builder.Property(e => e.PurchasedAiQuota)
            .HasDefaultValue(0);

        builder.Property(e => e.UsedAiQuota)
            .HasDefaultValue(0);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // UserId unique index — only for registered patients (non-null UserId)
        builder.HasIndex(e => e.UserId)
            .IsUnique()
            .HasFilter("\"UserId\" IS NOT NULL");

        // Relationships
        builder.HasMany(e => e.RetinalImages)
            .WithOne()
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.AiScreenings)
            .WithOne()
            .HasForeignKey(s => s.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Computed column — not mapped, EF ignores it
        builder.Ignore(e => e.IsWalkIn);
    }
}
