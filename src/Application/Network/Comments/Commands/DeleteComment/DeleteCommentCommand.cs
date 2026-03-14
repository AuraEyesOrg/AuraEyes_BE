using Application.Common.Interfaces;

namespace Application.Network.Comments.Commands.DeleteComment;

/// <summary>
/// Command to soft-delete a comment
/// </summary>
public record DeleteCommentCommand : ICommand
{
    public Guid CommentId { get; init; }
    public Guid AuthorId { get; init; }
}
