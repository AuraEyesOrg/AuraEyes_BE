using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Network.Common;
using Domain.Enums.Network;
using Domain.Repositories;

namespace Application.Network.Posts.Queries.GetFeed;

/// <summary>
/// Handler for GetFeedQuery - returns paginated feed of posts with author info
/// </summary>
public class GetFeedQueryHandler : IQueryHandler<GetFeedQuery, PagedResult<PostFeedDto>>
{
    private readonly IPostRepository _postRepository;
    private readonly IIdentityService _identityService;

    public GetFeedQueryHandler(IPostRepository postRepository, IIdentityService identityService)
    {
        _postRepository = postRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<PostFeedDto>>> Handle(
        GetFeedQuery request,
        CancellationToken cancellationToken)
    {
        var (posts, totalCount) = await _postRepository.GetFeedAsync(
            request.Category,
            request.SearchTerm,
            request.PageNumber,
            request.PageSize,
            cancellationToken,
            request.AuthorId);

        var postIds = posts.Select(p => p.Id).ToList();

        // Batch load user reactions and bookmark status
        var userReactions = await _postRepository.GetUserReactionsForPostsAsync(
            request.CurrentUserId, postIds, cancellationToken);
        var savedPostIds = await _postRepository.GetUserSavedPostIdsAsync(
            request.CurrentUserId, postIds, cancellationToken);

        // Build author info
        var authorIds = posts.Select(p => p.AuthorId).Distinct().ToList();
        var authors = new Dictionary<Guid, AuthorDto>();
        foreach (var authorId in authorIds)
        {
            var user = await _identityService.GetUserByIdAsync(authorId, cancellationToken);
            authors[authorId] = new AuthorDto
            {
                Id = authorId,
                AuthorType = posts.First(p => p.AuthorId == authorId).AuthorType,
                FullName = user?.FullName ?? "Unknown",
                AvatarUrl = null
            };
        }

        var items = posts.Select(p => new PostFeedDto
        {
            Id = p.Id,
            Author = authors.GetValueOrDefault(p.AuthorId) ?? new AuthorDto { Id = p.AuthorId, FullName = "Unknown" },
            Content = p.Content,
            Category = p.Category,
            IsRepost = p.IsRepost,
            RepostComment = p.RepostComment,
            OriginalPostId = p.OriginalPostId,
            ReactionCount = p.ReactionCount,
            CommentCount = p.CommentCount,
            RepostCount = p.RepostCount,
            ViewCount = p.ViewCount,
            AllowComments = p.AllowComments,
            Attachments = p.Attachments.Select(a => new AttachmentDto
            {
                Id = a.Id,
                Type = a.Type,
                FileName = a.FileName,
                FileUrl = a.FileUrl,
                MimeType = a.MimeType,
                FileSize = a.FileSize,
                DisplayOrder = a.DisplayOrder
            }).OrderBy(a => a.DisplayOrder).ToList(),
            CurrentUserReaction = userReactions.GetValueOrDefault(p.Id),
            IsBookmarked = savedPostIds.Contains(p.Id),
            CreatedAt = p.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<PostFeedDto>(
            items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<PostFeedDto>>.Success(pagedResult);
    }
}
