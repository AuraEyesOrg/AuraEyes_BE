using Application.Common.Interfaces;

namespace Application.Network.Posts.Queries.GetPostById;

/// <summary>
/// Query to get a single post by ID with full details
/// </summary>
public record GetPostByIdQuery : IQuery<PostDetailDto>
{
    public Guid PostId { get; init; }
    public Guid CurrentUserId { get; init; }
    public bool IsSystemAdmin { get; init; }
}
