using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Network.SavedPosts.Queries.GetSavedPosts;

/// <summary>
/// Query to get user's saved posts
/// </summary>
public record GetSavedPostsQuery : IQuery<PagedResult<SavedPostDto>>
{
    public Guid UserId { get; init; }
    public string? CollectionName { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
