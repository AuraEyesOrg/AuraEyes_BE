using Domain.Common;

namespace Domain.Entities.Network;

/// <summary>
/// Saved/bookmarked post entity
/// </summary>
public class SavedPost : BaseEntity
{
    /// <summary>
    /// User who saved the post (NO FK - just stores ID)
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Post that was saved
    /// </summary>
    public Guid PostId { get; private set; }

    /// <summary>
    /// Optional collection/folder name
    /// </summary>
    public string? CollectionName { get; private set; }

    // Navigation property (within network module only)
    public virtual ProfessionalPost Post { get; private set; } = null!;

    private SavedPost() { } // EF Core

    /// <summary>
    /// Save a post
    /// </summary>
    public SavedPost(Guid userId, Guid postId, string? collectionName = null)
    {
        UserId = userId;
        PostId = postId;
        CollectionName = collectionName;
    }

    /// <summary>
    /// Move to a different collection
    /// </summary>
    public void MoveToCollection(string? collectionName)
    {
        CollectionName = collectionName;
        UpdatedAt = DateTime.UtcNow;
    }
}
