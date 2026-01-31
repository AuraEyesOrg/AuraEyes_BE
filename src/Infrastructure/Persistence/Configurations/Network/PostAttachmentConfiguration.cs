using Domain.Entities.Network;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Network;

public class PostAttachmentConfiguration : IEntityTypeConfiguration<PostAttachment>
{
    public void Configure(EntityTypeBuilder<PostAttachment> builder)
    {
        builder.ToTable("PostAttachments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.PostId)
            .IsRequired();

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.FileUrl)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.MimeType)
            .HasMaxLength(100);

        builder.Property(e => e.DisplayOrder)
            .HasDefaultValue(0);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Indexes for performance
        builder.HasIndex(e => e.PostId);
    }
}
