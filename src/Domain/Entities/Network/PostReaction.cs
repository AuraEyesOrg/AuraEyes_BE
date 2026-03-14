using Domain.Common;
using Domain.Enums.Network;

namespace Domain.Entities.Network;

/// <summary>
/// Reaction on a professional post
/// </summary>
public class PostReaction : BaseEntity
{
    /// <summary>
    /// Post that was reacted to
    /// </summary>
    public Guid PostId { get; private set; }
    
    /// <summary>
    /// User who reacted (NO FK - just stores ID)
    /// </summary>
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// Type of reaction
    /// </summary>
    public ReactionType Type { get; private set; }

    // Navigation property (within network module only)
    public virtual ProfessionalPost Post { get; private set; } = null!;

    private PostReaction() { } // EF Core

    /// <summary>
    /// Create a reaction
    /// </summary>
    public PostReaction(Guid postId, Guid userId, ReactionType type)
    {
        PostId = postId;
        UserId = userId;
        Type = type;
    }

    /// <summary>
    /// Change reaction type
    /// </summary>
    public void ChangeType(ReactionType newType)
    {
        Type = newType;
        UpdatedAt = DateTime.UtcNow;
    }
}
