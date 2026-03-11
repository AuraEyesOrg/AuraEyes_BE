using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Network.Posts.Commands.UpdatePost;

/// <summary>
/// Handler for UpdatePostCommand
/// </summary>
public class UpdatePostCommandHandler : ICommandHandler<UpdatePostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdatePostCommand request,
        CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result.NotFound("Post not found");

        if (post.AuthorId != request.AuthorId)
            return Result.Forbidden("You can only edit your own posts");

        post.UpdateContent(request.Content);

        if (request.AllowComments.HasValue)
        {
            post.UpdateAllowComments(request.AllowComments.Value);
        }

        await _postRepository.UpdateAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
