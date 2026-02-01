using Domain.Entities.Network;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Network;

public class PostReactionConfiguration : IEntityTypeConfiguration<PostReaction>
{
    public void Configure(EntityTypeBuilder<PostReaction> builder)
    {
        builder.ToTable("PostReactions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.PostId)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Unique constraint: one user can only have one reaction per post
        builder.HasIndex(e => new { e.PostId, e.UserId })
            .IsUnique();

        // Indexes for performance
        builder.HasIndex(e => e.PostId);
        builder.HasIndex(e => e.UserId);
    }
}
