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
            .Select(p => p.AuthorId)
            .Concat(posts.Where(p => p.OriginalPost != null).Select(p => p.OriginalPost!.AuthorId))
            .Distinct()
            .ToList();

        var users = await _identityService.GetUsersByIdsAsync(authorIds, cancellationToken);
        var userDict = users.ToDictionary(u => u.Id);

        // Map author details from posts to avoid repeated FirstOrDefault in the loop
        var authorTypeMap = new Dictionary<Guid, AuthorType>();
        foreach (var p in posts)
        {
            authorTypeMap.TryAdd(p.AuthorId, p.AuthorType);
            if (p.OriginalPost != null)
            {
                authorTypeMap.TryAdd(p.OriginalPost.AuthorId, p.OriginalPost.AuthorType);
            }
        }

        return authorIds.ToDictionary(
            id => id,
            id => new AuthorDto
            {
                Id = id,
                AuthorType = authorTypeMap.GetValueOrDefault(id, AuthorType.Ophthalmologist),
                FullName = userDict.GetValueOrDefault(id)?.FullName ?? "Unknown",
                AvatarUrl = userDict.GetValueOrDefault(id)?.AvatarUrl
            });
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
            OriginalPost = MapOriginalPost(p.OriginalPost, authors, currentUserId, isSystemAdmin),
            ReactionCount = p.ReactionCount,
            CommentCount = p.CommentCount,
            RepostCount = p.RepostCount,
            ViewCount = p.ViewCount,
            AllowComments = p.AllowComments,
            IsInternalCase = p.IsInternalCase,
            ConsultationSessionId = p.ConsultationSessionId,
            AiScreeningId = (p.AuthorId == currentUserId || isSystemAdmin) ? p.AiScreeningId : null,
            PatientAge = p.PatientAge,
            PatientGender = p.PatientGender,
            Attachments = MapAttachments(p.Attachments),
            CurrentUserReaction = userReactions.GetValueOrDefault(p.Id),
            IsBookmarked = savedPostIds.Contains(p.Id),
            IsHidden = p.IsHidden,
            HideReason = p.IsHidden ? p.HideReason : null,
            CreatedAt = p.CreatedAt
        };
    }

    private static OriginalPostDto? MapOriginalPost(Domain.Entities.Network.ProfessionalPost? op, Dictionary<Guid, AuthorDto> authors, Guid currentUserId, bool isSystemAdmin)
    {
        if (op == null) return null;

        var isRestricted = op.IsHidden && op.AuthorId != currentUserId && !isSystemAdmin;

        return new OriginalPostDto
        {
            Id = op.Id,
            Author = authors.GetValueOrDefault(op.AuthorId) ?? new AuthorDto { Id = op.AuthorId, FullName = "Unknown" },
            Content = isRestricted ? "This original post is hidden by moderators." : op.Content,
            Category = op.Category,
            Attachments = isRestricted ? new List<AttachmentDto>() : MapAttachments(op.Attachments),
            IsHidden = op.IsHidden,
            HideReason = op.IsHidden ? op.HideReason : null,
            CreatedAt = op.CreatedAt
        };
    }

    private static List<AttachmentDto> MapAttachments(IEnumerable<Domain.Entities.Network.PostAttachment> attachments)
    {
        return attachments.Select(a => new AttachmentDto
        {
            Id = a.Id,
            Type = a.Type,
            FileName = a.FileName,
            FileUrl = a.FileUrl,
            MimeType = a.MimeType,
            FileSize = a.FileSize,
            DisplayOrder = a.DisplayOrder
        }).OrderBy(a => a.DisplayOrder).ToList();
    }
}
