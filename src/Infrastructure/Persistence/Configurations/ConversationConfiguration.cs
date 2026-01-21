using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne<Ophthalmologist>()
            .WithMany()
            .HasForeignKey(e => e.OphthalmologistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ConsultationRequest>()
            .WithMany(c => c.Conversations)
            .HasForeignKey(e => e.ConsultationRequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
