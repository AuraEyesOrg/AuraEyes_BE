using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network;
using Domain.Repositories;

namespace Application.Network.Comments.Commands.CreateComment;

/// <summary>
/// Handler for CreateCommentCommand
/// </summary>
public class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, Guid>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommentCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateCommentCommand request,
        CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result<Guid>.NotFound("Post not found");

        if (!post.AllowComments)
            return Result<Guid>.Failure("Comments are disabled for this post");

        // If replying to a comment, verify parent exists
        if (request.ParentCommentId.HasValue)
        {
            var parentComment = await _postRepository.GetCommentByIdAsync(
                request.ParentCommentId.Value, cancellationToken);
            if (parentComment == null)
                return Result<Guid>.NotFound("Parent comment not found");

            parentComment.IncrementReplyCount();
        }

        var comment = new PostComment(
            request.PostId,
            request.AuthorId,
            request.AuthorType,
            request.Content,
            request.ParentCommentId);

        await _postRepository.AddCommentAsync(comment, cancellationToken);
        post.IncrementCommentCount();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(comment.Id);
    }
}
