using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network;
using Domain.Repositories;

namespace Application.Network.Reactions.Commands.ToggleReaction;

/// <summary>
/// Handler for ToggleReactionCommand - toggles reaction on a post
/// If same reaction exists, removes it. If different, changes it. If none, adds it.
/// </summary>
public class ToggleReactionCommandHandler : ICommandHandler<ToggleReactionCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleReactionCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ToggleReactionCommand request,
        CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result.NotFound("Post not found");

        var existing = await _postRepository.GetReactionAsync(
            request.PostId, request.UserId, cancellationToken);

        if (existing != null)
        {
            if (existing.Type == request.Type)
            {
                // Same reaction - remove it
                await _postRepository.RemoveReactionAsync(existing, cancellationToken);
                post.DecrementReactionCount();
            }
            else
            {
                // Different reaction - change it
                existing.ChangeType(request.Type);
            }
        }
        else
        {
            // No existing reaction - add new
            var reaction = new PostReaction(request.PostId, request.UserId, request.Type);
            await _postRepository.AddReactionAsync(reaction, cancellationToken);
            post.IncrementReactionCount();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
