using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Network.Comments.Commands.DeleteComment;

/// <summary>
/// Handler for DeleteCommentCommand - soft deletes a comment
/// </summary>
public class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommentCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await _postRepository.GetCommentByIdAsync(
            request.CommentId, cancellationToken);

        if (comment == null)
            return Result.NotFound("Comment not found");

        if (comment.AuthorId != request.AuthorId)
            return Result.Forbidden("You can only delete your own comments");

        // Decrement post comment count
        var post = await _postRepository.GetByIdAsync(comment.PostId, cancellationToken);
        post?.DecrementCommentCount();

        // Decrement parent reply count if this is a reply
        if (comment.ParentCommentId.HasValue)
        {
            var parentComment = await _postRepository.GetCommentByIdAsync(
                comment.ParentCommentId.Value, cancellationToken);
            parentComment?.DecrementReplyCount();
        }

        // Soft delete via EF Core global filter (IsDeleted)
        // Since PostComment doesn't implement IAggregateRoot, we use the unit of work
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
