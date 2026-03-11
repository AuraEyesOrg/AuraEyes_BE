using Application.Network.Common;
using Domain.Enums.Network;

namespace Application.Network.Posts.Queries.GetFeed;

/// <summary>
/// Post item DTO for feed listing
/// </summary>
public class PostFeedDto
{
    public Guid Id { get; set; }
    public AuthorDto Author { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public PostCategory Category { get; set; }
    public bool IsRepost { get; set; }
    public string? RepostComment { get; set; }
    public Guid? OriginalPostId { get; set; }

    /// <summary>
    /// Nested original post data for quote/repost display.
    /// Populated only when IsRepost = true.
    /// </summary>
    public OriginalPostDto? OriginalPost { get; set; }

    public int ReactionCount { get; set; }
    public int CommentCount { get; set; }
    public int RepostCount { get; set; }
    public int ViewCount { get; set; }
    public bool AllowComments { get; set; }
    public List<AttachmentDto> Attachments { get; set; } = new();
    public ReactionType? CurrentUserReaction { get; set; }
    public bool IsBookmarked { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Compact representation of the original post shown inside a repost card.
/// </summary>
public class OriginalPostDto
{
    public Guid Id { get; set; }
    public AuthorDto Author { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public PostCategory Category { get; set; }
    public List<AttachmentDto> Attachments { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
