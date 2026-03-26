using Domain.Common;
using Domain.Enums.Network;

namespace Domain.Entities.Network;

/// <summary>
/// Comment on a professional post
/// Supports nested comments (replies)
/// </summary>
public class PostComment : BaseEntity
{
    /// <summary>
    /// Post that was commented on
    /// </summary>
    public Guid PostId { get; private set; }

    /// <summary>
    /// Author ID (NO FK - just stores ID)
    /// </summary>
    public Guid AuthorId { get; private set; }

    /// <summary>
    /// Type of author
    /// </summary>
    public AuthorType AuthorType { get; private set; }

    /// <summary>
    /// Comment content
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Parent comment ID for nested replies (self-reference)
    /// </summary>
    public Guid? ParentCommentId { get; private set; }

    /// <summary>
    /// Number of replies (denormalized)
    /// </summary>
    public int ReplyCount { get; private set; }

    /// <summary>
    /// Number of likes (denormalized)
    /// </summary>
    public int LikeCount { get; private set; }

    // Navigation properties (within network module only)
    public virtual ProfessionalPost Post { get; private set; } = null!;
    public virtual PostComment? ParentComment { get; private set; }

    private readonly List<PostComment> _replies = new();
    public virtual IReadOnlyCollection<PostComment> Replies => _replies.AsReadOnly();

    private PostComment() { } // EF Core

    /// <summary>
    /// Create a comment
    /// </summary>
    public PostComment(Guid postId, Guid authorId, AuthorType authorType, string content, Guid? parentCommentId = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content cannot be empty", nameof(content));

        PostId = postId;
        AuthorId = authorId;
        AuthorType = authorType;
        Content = content;
        ParentCommentId = parentCommentId;
    }

    /// <summary>
    /// Update comment content
    /// </summary>
    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content cannot be empty", nameof(content));

        Content = content;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Increment reply count
    /// </summary>
    public void IncrementReplyCount()
    {
        ReplyCount++;
    }

    /// <summary>
    /// Decrement reply count
    /// </summary>
    public void DecrementReplyCount()
    {
        if (ReplyCount > 0)
            ReplyCount--;
    }

    /// <summary>
    /// Increment like count
    /// </summary>
    public void IncrementLikeCount()
    {
        LikeCount++;
    }

    /// <summary>
    /// Decrement like count
    /// </summary>
    public void DecrementLikeCount()
    {
        if (LikeCount > 0)
            LikeCount--;
    }
}
