using Domain.Entities.Network;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Network;

public class ProfessionalPostConfiguration : IEntityTypeConfiguration<ProfessionalPost>
{
    public void Configure(EntityTypeBuilder<ProfessionalPost> builder)
    {
        builder.ToTable("ProfessionalPosts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.AuthorId)
            .IsRequired();

        builder.Property(e => e.AuthorType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(e => e.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.RepostComment)
            .HasMaxLength(500);

        builder.Property(e => e.Visibility)
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(Domain.Enums.Network.PostVisibility.Public);

        builder.Property(e => e.AllowComments)
            .HasDefaultValue(true);

        builder.Property(e => e.IsRepost)
            .HasDefaultValue(false);

        builder.Property(e => e.ReactionCount)
            .HasDefaultValue(0);

        builder.Property(e => e.CommentCount)
            .HasDefaultValue(0);

        builder.Property(e => e.RepostCount)
            .HasDefaultValue(0);

        builder.Property(e => e.ViewCount)
            .HasDefaultValue(0);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Self-reference for reposts
        builder.HasOne(e => e.OriginalPost)
            .WithMany(e => e.Reposts)
            .HasForeignKey(e => e.OriginalPostId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relationships within network module
        builder.HasMany(e => e.Reactions)
            .WithOne(e => e.Post)
            .HasForeignKey(e => e.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Comments)
            .WithOne(e => e.Post)
            .HasForeignKey(e => e.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Attachments)
            .WithOne(e => e.Post)
            .HasForeignKey(e => e.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(e => e.AuthorId);
        builder.HasIndex(e => e.OrganisationId);
        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => new { e.AuthorId, e.AuthorType });
    }
}
