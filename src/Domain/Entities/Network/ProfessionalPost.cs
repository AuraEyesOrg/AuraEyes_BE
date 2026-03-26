using Domain.Common;
using Domain.Enums.Network;

namespace Domain.Entities.Network;

/// <summary>
/// Professional network post entity
/// Supports original posts and reposts
/// </summary>
public class ProfessionalPost : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Author ID - Ophthalmologist.Id or Organisation.Id (NO FK - just stores ID)
    /// </summary>
    public Guid AuthorId { get; private set; }

    /// <summary>
    /// Type of author
    /// </summary>
    public AuthorType AuthorType { get; private set; }

    /// <summary>
    /// Organisation context - if posting on behalf of organisation
    /// </summary>
    public Guid? OrganisationId { get; private set; }

    /// <summary>
    /// Post content
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Post category
    /// </summary>
    public PostCategory Category { get; private set; }

    /// <summary>
    /// Original post ID for reposts (self-reference)
    /// </summary>
    public Guid? OriginalPostId { get; private set; }

    /// <summary>
    /// Whether this is a repost
    /// </summary>
    public bool IsRepost { get; private set; }

    /// <summary>
    /// Comment added when reposting
    /// </summary>
    public string? RepostComment { get; private set; }

    /// <summary>
    /// Reaction count (denormalized for performance)
    /// </summary>
    public int ReactionCount { get; private set; }

    /// <summary>
    /// Comment count (denormalized for performance)
    /// </summary>
    public int CommentCount { get; private set; }

    /// <summary>
    /// Repost count (denormalized for performance)
    /// </summary>
    public int RepostCount { get; private set; }

    /// <summary>
    /// View count (denormalized for performance)
    /// </summary>
    public int ViewCount { get; private set; }

    /// <summary>
    /// Whether comments are allowed
    /// </summary>
    public bool AllowComments { get; private set; } = true;

    // Navigation properties (within network module only)
    public virtual ProfessionalPost? OriginalPost { get; private set; }

    private readonly List<ProfessionalPost> _reposts = new();
    public virtual IReadOnlyCollection<ProfessionalPost> Reposts => _reposts.AsReadOnly();

    private readonly List<PostReaction> _reactions = new();
    public virtual IReadOnlyCollection<PostReaction> Reactions => _reactions.AsReadOnly();

    private readonly List<PostComment> _comments = new();
    public virtual IReadOnlyCollection<PostComment> Comments => _comments.AsReadOnly();

    private readonly List<PostAttachment> _attachments = new();
    public virtual IReadOnlyCollection<PostAttachment> Attachments => _attachments.AsReadOnly();

    private ProfessionalPost() { } // EF Core

    /// <summary>
    /// Create a new post
    /// </summary>
    public ProfessionalPost(
        Guid authorId,
        AuthorType authorType,
        string content,
        PostCategory category,
        Guid? organisationId = null,
        bool allowComments = true)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Post content cannot be empty", nameof(content));

        AuthorId = authorId;
        AuthorType = authorType;
        Content = content;
        Category = category;
        OrganisationId = organisationId;
        AllowComments = allowComments;
        IsRepost = false;
    }

    /// <summary>
    /// Create a repost
    /// </summary>
    public static ProfessionalPost CreateRepost(
        Guid authorId,
        AuthorType authorType,
        ProfessionalPost originalPost,
        string? comment = null)
    {
        var repost = new ProfessionalPost
        {
            AuthorId = authorId,
            AuthorType = authorType,
            Content = originalPost.Content,
            Category = originalPost.Category,
            OriginalPostId = originalPost.Id,
            IsRepost = true,
            RepostComment = comment,
            AllowComments = true
        };

        return repost;
    }

    /// <summary>
    /// Update post content
    /// </summary>
    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Post content cannot be empty", nameof(content));

        Content = content;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update comment settings
    /// </summary>
    public void UpdateAllowComments(bool allowComments)
    {
        AllowComments = allowComments;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Increment reaction count
    /// </summary>
    public void IncrementReactionCount()
    {
        ReactionCount++;
    }

    /// <summary>
    /// Decrement reaction count
    /// </summary>
    public void DecrementReactionCount()
    {
        if (ReactionCount > 0)
            ReactionCount--;
    }

    /// <summary>
    /// Increment comment count
    /// </summary>
    public void IncrementCommentCount()
    {
        CommentCount++;
    }

    /// <summary>
    /// Decrement comment count
    /// </summary>
    public void DecrementCommentCount()
    {
        if (CommentCount > 0)
            CommentCount--;
    }

    /// <summary>
    /// Increment repost count
    /// </summary>
    public void IncrementRepostCount()
    {
        RepostCount++;
    }

    /// <summary>
    /// Increment view count
    /// </summary>
    public void IncrementViewCount()
    {
        ViewCount++;
    }

    /// <summary>
    /// Add attachment to post
    /// </summary>
    public void AddAttachment(PostAttachment attachment)
    {
        _attachments.Add(attachment);
        UpdatedAt = DateTime.UtcNow;
    }
}
