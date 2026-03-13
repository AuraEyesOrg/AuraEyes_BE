using Domain.Entities.Consultation;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WebsiteFeedbackConfiguration : IEntityTypeConfiguration<WebsiteFeedback>
{
    public void Configure(EntityTypeBuilder<WebsiteFeedback> builder)
    {
        builder.ToTable("WebsiteFeedback", tableBuilder =>
            tableBuilder.HasCheckConstraint("CK_WebsiteFeedback_Rating", "rating >= 1 AND rating <= 5"));

        builder.Property(e => e.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(e => e.Rating)
            .HasColumnName("rating")
            .IsRequired();

        builder.Property(e => e.Category)
            .HasColumnName("category")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.Comment)
            .HasColumnName("comment")
            .HasMaxLength(2000);

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(e => e.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(e => e.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        builder.Property(e => e.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.CreatedAt);

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
