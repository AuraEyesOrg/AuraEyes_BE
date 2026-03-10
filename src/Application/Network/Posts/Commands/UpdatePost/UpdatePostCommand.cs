using Application.Common.Interfaces;
using Domain.Enums.Network;

namespace Application.Network.Posts.Commands.UpdatePost;

/// <summary>
/// Command to update an existing post's content
/// </summary>
public record UpdatePostCommand : ICommand
{
    public Guid PostId { get; init; }
    public Guid AuthorId { get; init; }
    public string Content { get; init; } = string.Empty;
    public PostVisibility? Visibility { get; init; }
    public bool? AllowComments { get; init; }
}
