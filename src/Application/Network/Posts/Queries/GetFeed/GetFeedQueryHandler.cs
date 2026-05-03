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
            request.AuthorId,
            request.AuthorType,
            request.CurrentUserId,
            request.IsSystemAdmin,
            request.HiddenOnly && request.IsSystemAdmin);

        var postIds = posts.Select(p => p.Id).ToList();

        // Batch load user reactions and bookmark status
        var userReactions = await _postRepository.GetUserReactionsForPostsAsync(
            request.CurrentUserId, postIds, cancellationToken);
        var savedPostIds = await _postRepository.GetUserSavedPostIdsAsync(
            request.CurrentUserId, postIds, cancellationToken);

        var authors = await BatchLoadAuthorsAsync(posts, cancellationToken);

        var items = posts.Select(p => MapToDto(
            p, authors, userReactions, savedPostIds, request.CurrentUserId, request.IsSystemAdmin)).ToList();

        var pagedResult = new PagedResult<PostFeedDto>(
            items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<PostFeedDto>>.Success(pagedResult);
    }

    private async Task<Dictionary<Guid, AuthorDto>> BatchLoadAuthorsAsync(
        IReadOnlyList<Domain.Entities.Network.ProfessionalPost> posts,
        CancellationToken cancellationToken)
    {
        var authorIds = posts
            .SelectMany(p => p.OriginalPost is not null
                ? new[] { p.AuthorId, p.OriginalPost.AuthorId }
                : new[] { p.AuthorId })
            .Distinct()
            .ToList();

        var authors = new Dictionary<Guid, AuthorDto>();
        foreach (var authorId in authorIds)
        {
            var user = await _identityService.GetUserByIdAsync(authorId, cancellationToken);
            var matchingPost = posts.FirstOrDefault(p => p.AuthorId == authorId)
                               ?? posts.FirstOrDefault(p => p.OriginalPost?.AuthorId == authorId);
            
            authors[authorId] = new AuthorDto
            {
                Id = authorId,
                AuthorType = matchingPost?.AuthorId == authorId
                    ? matchingPost.AuthorType
                    : matchingPost!.OriginalPost!.AuthorType,
                FullName = user?.FullName ?? "Unknown",
                AvatarUrl = user?.AvatarUrl
            };
        }
        return authors;
    }

    private static PostFeedDto MapToDto(
        Domain.Entities.Network.ProfessionalPost p,
        Dictionary<Guid, AuthorDto> authors,
        Dictionary<Guid, Domain.Enums.Network.ReactionType> userReactions,
        HashSet<Guid> savedPostIds,
        Guid currentUserId,
        bool isSystemAdmin)
    {
        return new PostFeedDto
        {
            Id = p.Id,
            Author = authors.GetValueOrDefault(p.AuthorId) ?? new AuthorDto { Id = p.AuthorId, FullName = "Unknown" },
            Content = p.Content,
            Category = p.Category,
            IsRepost = p.IsRepost,
            RepostComment = p.RepostComment,
            OriginalPostId = p.OriginalPostId,
            OriginalPost = p.IsRepost && p.OriginalPost is not null
                ? new OriginalPostDto
                {
                    Id = p.OriginalPost.Id,
                    Author = authors.GetValueOrDefault(p.OriginalPost.AuthorId)
                             ?? new AuthorDto { Id = p.OriginalPost.AuthorId, FullName = "Unknown" },
                    Content = p.OriginalPost.IsHidden && p.OriginalPost.AuthorId != currentUserId && !isSystemAdmin
                        ? "This original post is hidden by moderators."
                        : p.OriginalPost.Content,
                    Category = p.OriginalPost.Category,
                    Attachments = p.OriginalPost.IsHidden && p.OriginalPost.AuthorId != currentUserId && !isSystemAdmin
                        ? new List<AttachmentDto>()
                        : p.OriginalPost.Attachments.Select(a => new AttachmentDto
                        {
                            Id = a.Id,
                            Type = a.Type,
                            FileName = a.FileName,
                            FileUrl = a.FileUrl,
                            MimeType = a.MimeType,
                            FileSize = a.FileSize,
                            DisplayOrder = a.DisplayOrder
                        }).OrderBy(a => a.DisplayOrder).ToList(),
                    IsHidden = p.OriginalPost.IsHidden,
                    HideReason = p.OriginalPost.IsHidden ? p.OriginalPost.HideReason : null,
                    CreatedAt = p.OriginalPost.CreatedAt
                }
                : null,
            ReactionCount = p.ReactionCount,
            CommentCount = p.CommentCount,
            RepostCount = p.RepostCount,
            ViewCount = p.ViewCount,
            AllowComments = p.AllowComments,
            IsInternalCase = p.IsInternalCase,
            ConsultationSessionId = p.ConsultationSessionId,
            AiScreeningId = p.AuthorId == currentUserId || isSystemAdmin
                ? p.AiScreeningId
                : null,
            PatientAge = p.PatientAge,
            PatientGender = p.PatientGender,
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
            IsHidden = p.IsHidden,
            HideReason = p.IsHidden ? p.HideReason : null,
            CreatedAt = p.CreatedAt
        };
    }
}
