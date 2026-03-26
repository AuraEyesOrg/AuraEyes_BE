using Application.Network.Common;
using Domain.Enums.Network;

namespace Application.Network.Posts.Queries.GetPostById;

/// <summary>
/// Detailed post DTO including author info and attachments
/// </summary>
public class PostDetailDto
{
    public Guid Id { get; set; }
    public AuthorDto Author { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public PostCategory Category { get; set; }
    public bool IsRepost { get; set; }
    public string? RepostComment { get; set; }
    public Guid? OriginalPostId { get; set; }
    public int ReactionCount { get; set; }
    public int CommentCount { get; set; }
    public int RepostCount { get; set; }
    public int ViewCount { get; set; }
    public bool AllowComments { get; set; }
    public List<AttachmentDto> Attachments { get; set; } = new();
    public ReactionType? CurrentUserReaction { get; set; }
    public bool IsBookmarked { get; set; }
    public bool IsHidden { get; set; }
    public string? HideReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
