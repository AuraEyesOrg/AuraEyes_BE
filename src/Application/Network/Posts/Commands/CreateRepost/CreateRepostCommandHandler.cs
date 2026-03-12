using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network;
using Domain.Enums.Network;
using Domain.Repositories;

namespace Application.Network.Posts.Commands.CreateRepost;

/// <summary>
/// Handler for CreateRepostCommand.
/// Validates the original post exists, creates a new repost record,
/// and increments the original post's repost counter (denormalized).
/// </summary>
public class CreateRepostCommandHandler : ICommandHandler<CreateRepostCommand, Guid>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRepostCommandHandler(
        IPostRepository postRepository,
        IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateRepostCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Verify the original post exists
        var originalPost = await _postRepository.GetByIdWithAttachmentsAsync(
            request.OriginalPostId, cancellationToken);

        if (originalPost is null)
            return Result<Guid>.Failure("Bài viết gốc không tồn tại hoặc đã bị xóa.");

        // 2. Prevent self-repost (cannot share your own post)
        if (originalPost.AuthorId == request.AuthorId)
            return Result<Guid>.Failure("Không thể tự chia sẻ bài viết của mình.");

        // 3. Prevent reposts of reposts (quote-of-quote not allowed)
        if (originalPost.IsRepost)
            return Result<Guid>.Failure("Không thể chia sẻ lại một bài viết đã được chia sẻ.");

        // 3. Create the repost using the domain factory method
        var repost = ProfessionalPost.CreateRepost(
            request.AuthorId,
            request.AuthorType,
            originalPost,
            request.RepostComment);

        // 4. Persist the repost
        await _postRepository.AddAsync(repost, cancellationToken);

        // 5. Increment denormalized repost counter on original post
        originalPost.IncrementRepostCount();

        // 6. Save both changes atomically
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(repost.Id);
    }
}
