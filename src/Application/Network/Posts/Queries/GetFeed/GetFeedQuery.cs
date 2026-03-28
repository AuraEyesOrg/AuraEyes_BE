using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums.Network;

namespace Application.Network.Posts.Queries.GetFeed;

/// <summary>
/// Query to get paginated feed of professional posts
/// </summary>
public record GetFeedQuery : IQuery<PagedResult<PostFeedDto>>
{
    public Guid CurrentUserId { get; init; }
    public bool IsSystemAdmin { get; init; }
    public PostCategory? Category { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public bool HiddenOnly { get; init; }
    /// <summary>
    /// Optional: filter posts by a specific author (used on profile pages)
    /// </summary>
    public Guid? AuthorId { get; init; }
    /// <summary>
    /// Optional: filter by author type (Ophthalmologist or Organisation)
    /// </summary>
    public AuthorType? AuthorType { get; init; }
}
