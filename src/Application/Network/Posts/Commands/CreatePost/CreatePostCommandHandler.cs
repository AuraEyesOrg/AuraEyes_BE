using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network;
using Domain.Repositories;

namespace Application.Network.Posts.Commands.CreatePost;

/// <summary>
/// Handler for CreatePostCommand
/// </summary>
public class CreatePostCommandHandler : ICommandHandler<CreatePostCommand, Guid>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreatePostCommand request,
        CancellationToken cancellationToken)
    {
        var post = new ProfessionalPost(
            request.AuthorId,
            request.AuthorType,
            request.Content,
            request.Category,
            request.OrganisationId,
            request.Visibility,
            request.AllowComments);

        await _postRepository.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(post.Id);
    }
}
