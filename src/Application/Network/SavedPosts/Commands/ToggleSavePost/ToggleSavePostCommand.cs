using Application.Common.Interfaces;

namespace Application.Network.SavedPosts.Commands.ToggleSavePost;

/// <summary>
/// Command to save or unsave a post
/// </summary>
public record ToggleSavePostCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid PostId { get; init; }
    public string? CollectionName { get; init; }
}
