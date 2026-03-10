using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Network.Comments.Queries.GetComments;

/// <summary>
/// Query to get comments for a post with pagination
/// </summary>
public record GetCommentsQuery : IQuery<PagedResult<CommentDto>>
{
    public Guid PostId { get; init; }
    public Guid? ParentCommentId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
