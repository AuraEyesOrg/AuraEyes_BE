using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network;
using Domain.Repositories;

namespace Application.Network.SavedPosts.Commands.ToggleSavePost;

/// <summary>
/// Handler for ToggleSavePostCommand - saves or unsaves a post
/// </summary>
public class ToggleSavePostCommandHandler : ICommandHandler<ToggleSavePostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleSavePostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ToggleSavePostCommand request,
        CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result.NotFound("Post not found");

        var existing = await _postRepository.GetSavedPostAsync(
            request.UserId, request.PostId, cancellationToken);

        if (existing != null)
        {
            // Already saved - unsave
            await _postRepository.RemoveSavedPostAsync(existing, cancellationToken);
        }
        else
        {
            // Not saved - save
            var savedPost = new SavedPost(request.UserId, request.PostId, request.CollectionName);
            await _postRepository.AddSavedPostAsync(savedPost, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
