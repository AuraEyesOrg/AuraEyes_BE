using Application.Common.Interfaces;
using Domain.Enums.Network;

namespace Application.Network.Comments.Commands.CreateComment;

/// <summary>
/// Command to create a comment on a post
/// </summary>
public record CreateCommentCommand : ICommand<Guid>
{
    public Guid PostId { get; init; }
    public Guid AuthorId { get; init; }
    public AuthorType AuthorType { get; init; }
    public string Content { get; init; } = string.Empty;
    public Guid? ParentCommentId { get; init; }
}
