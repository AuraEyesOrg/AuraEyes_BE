using Domain.Entities.Network;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Network;

public class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
{
    public void Configure(EntityTypeBuilder<PostComment> builder)
    {
        builder.ToTable("PostComments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.PostId)
            .IsRequired();

        builder.Property(e => e.AuthorId)
            .IsRequired();

        builder.Property(e => e.AuthorType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.ReplyCount)
            .HasDefaultValue(0);

        builder.Property(e => e.LikeCount)
            .HasDefaultValue(0);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Self-reference for nested comments
        builder.HasOne(e => e.ParentComment)
            .WithMany(e => e.Replies)
            .HasForeignKey(e => e.ParentCommentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(e => e.PostId);
        builder.HasIndex(e => e.AuthorId);
        builder.HasIndex(e => e.ParentCommentId);
        builder.HasIndex(e => e.CreatedAt);
    }
}
