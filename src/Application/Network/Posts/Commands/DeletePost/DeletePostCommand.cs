using Application.Common.Interfaces;

namespace Application.Network.Posts.Commands.DeletePost;

/// <summary>
/// Command to soft-delete a post
/// </summary>
public record DeletePostCommand : ICommand
{
    public Guid PostId { get; init; }
    public Guid AuthorId { get; init; }
}
