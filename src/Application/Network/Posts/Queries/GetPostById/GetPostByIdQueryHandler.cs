using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Network.Common;
using Domain.Repositories;

namespace Application.Network.Posts.Queries.GetPostById;

/// <summary>
/// Handler for GetPostByIdQuery - returns full post details
/// </summary>
public class GetPostByIdQueryHandler : IQueryHandler<GetPostByIdQuery, PostDetailDto>
{
    private readonly IPostRepository _postRepository;
    private readonly IIdentityService _identityService;

    public GetPostByIdQueryHandler(IPostRepository postRepository, IIdentityService identityService)
    {
        _postRepository = postRepository;
        _identityService = identityService;
    }

    public async Task<Result<PostDetailDto>> Handle(
        GetPostByIdQuery request,
        CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdWithAttachmentsAsync(
            request.PostId, cancellationToken);

        if (post == null)
            return Result<PostDetailDto>.NotFound("Post not found");

        // Increment view count
        post.IncrementViewCount();

        // Get author info
        var user = await _identityService.GetUserByIdAsync(post.AuthorId, cancellationToken);
        var author = new AuthorDto
        {
            Id = post.AuthorId,
            AuthorType = post.AuthorType,
            FullName = user?.FullName ?? "Unknown",
            AvatarUrl = null
        };

        // Get user's reaction and bookmark status
        var reaction = await _postRepository.GetReactionAsync(
            request.PostId, request.CurrentUserId, cancellationToken);
        var isBookmarked = await _postRepository.IsPostSavedByUserAsync(
            request.CurrentUserId, request.PostId, cancellationToken);

        var dto = new PostDetailDto
        {
            Id = post.Id,
            Author = author,
            Content = post.Content,
            Category = post.Category,
            Visibility = post.Visibility,
            IsRepost = post.IsRepost,
            RepostComment = post.RepostComment,
            OriginalPostId = post.OriginalPostId,
            ReactionCount = post.ReactionCount,
            CommentCount = post.CommentCount,
            RepostCount = post.RepostCount,
            ViewCount = post.ViewCount,
            AllowComments = post.AllowComments,
            Attachments = post.Attachments.Select(a => new AttachmentDto
            {
                Id = a.Id,
                Type = a.Type,
                FileName = a.FileName,
                FileUrl = a.FileUrl,
                MimeType = a.MimeType,
                FileSize = a.FileSize,
                DisplayOrder = a.DisplayOrder
            }).OrderBy(a => a.DisplayOrder).ToList(),
            CurrentUserReaction = reaction?.Type,
            IsBookmarked = isBookmarked,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };

        return Result<PostDetailDto>.Success(dto);
    }
}
