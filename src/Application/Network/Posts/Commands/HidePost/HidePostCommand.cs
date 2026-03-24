using Application.Common.Interfaces;

namespace Application.Network.Posts.Commands.HidePost;

/// <summary>
/// Command to hide a post by moderation.
/// </summary>
public record HidePostCommand : ICommand
{
    public Guid PostId { get; init; }
    public string? HideReason { get; init; }
}