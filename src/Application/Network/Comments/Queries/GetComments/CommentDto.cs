using Application.Network.Common;

namespace Application.Network.Comments.Queries.GetComments;

/// <summary>
/// Comment DTO
/// </summary>
public class CommentDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public AuthorDto Author { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public int ReplyCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
