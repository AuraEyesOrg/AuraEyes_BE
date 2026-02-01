using Domain.Entities.Network;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Network;

public class SavedPostConfiguration : IEntityTypeConfiguration<SavedPost>
{
    public void Configure(EntityTypeBuilder<SavedPost> builder)
    {
        builder.ToTable("SavedPosts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.PostId)
            .IsRequired();

        builder.Property(e => e.CollectionName)
            .HasMaxLength(100);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationship with ProfessionalPost
        builder.HasOne(e => e.Post)
            .WithMany()
            .HasForeignKey(e => e.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: one user can only save a post once
        builder.HasIndex(e => new { e.UserId, e.PostId })
            .IsUnique();

        // Indexes for performance
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.PostId);
    }
}
