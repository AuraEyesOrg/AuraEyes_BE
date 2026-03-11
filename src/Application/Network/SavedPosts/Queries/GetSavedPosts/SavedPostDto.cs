using Application.Network.Posts.Queries.GetFeed;

namespace Application.Network.SavedPosts.Queries.GetSavedPosts;

/// <summary>
/// Saved post DTO
/// </summary>
public class SavedPostDto
{
    public Guid Id { get; set; }
    public string? CollectionName { get; set; }
    public DateTime SavedAt { get; set; }
    public PostFeedDto Post { get; set; } = null!;
}
