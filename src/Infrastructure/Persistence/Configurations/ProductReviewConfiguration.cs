using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("ProductReviews");

        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.ReviewerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pr => pr.Rating)
            .IsRequired();

        builder.Property(pr => pr.Comment)
            .HasMaxLength(500);

        builder.Property(pr => pr.CreatedAt)
            .IsRequired();

        builder.Ignore(pr => pr.DomainEvents);
    }
}
