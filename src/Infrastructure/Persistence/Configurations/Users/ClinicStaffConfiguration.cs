using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="ClinicStaff"/> entity.
/// Maps to the "ClinicStaffs" table.
/// </summary>
public class ClinicStaffConfiguration : IEntityTypeConfiguration<ClinicStaff>
{
    public void Configure(EntityTypeBuilder<ClinicStaff> builder)
    {
        builder.ToTable("ClinicStaffs");

        builder.Property(e => e.SubRoles)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Department)
            .HasMaxLength(100);

        builder.Property(e => e.EmployeeCode)
            .HasMaxLength(50);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // One staff member per Identity user (1:1)
        builder.HasIndex(e => e.UserId)
            .IsUnique();
    }
}
