using Application.Common.Interfaces;
using Domain.Enums.Network;

namespace Application.Network.Posts.Commands.CreatePost;

/// <summary>
/// Command to create a new professional post
/// </summary>
public record CreatePostCommand : ICommand<Guid>
{
    public Guid AuthorId { get; init; }
    public AuthorType AuthorType { get; init; }
    public string Content { get; init; } = string.Empty;
    public PostCategory Category { get; init; }
    public Guid? OrganisationId { get; init; }
    public PostVisibility Visibility { get; init; } = PostVisibility.Public;
    public bool AllowComments { get; init; } = true;
}
