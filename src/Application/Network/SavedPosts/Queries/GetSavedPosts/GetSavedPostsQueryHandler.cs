using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Network.Common;
using Application.Network.Posts.Queries.GetFeed;
using Domain.Repositories;

namespace Application.Network.SavedPosts.Queries.GetSavedPosts;

/// <summary>
/// Handler for GetSavedPostsQuery
/// </summary>
public class GetSavedPostsQueryHandler : IQueryHandler<GetSavedPostsQuery, PagedResult<SavedPostDto>>
{
    private readonly IPostRepository _postRepository;
    private readonly IIdentityService _identityService;

    public GetSavedPostsQueryHandler(IPostRepository postRepository, IIdentityService identityService)
    {
        _postRepository = postRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<SavedPostDto>>> Handle(
        GetSavedPostsQuery request,
        CancellationToken cancellationToken)
    {
        var (savedPosts, totalCount) = await _postRepository.GetSavedPostsAsync(
            request.UserId,
            request.CollectionName,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        // Build author info for posts
        var authorIds = savedPosts.Select(s => s.Post.AuthorId).Distinct().ToList();
        var authors = new Dictionary<Guid, AuthorDto>();
        foreach (var authorId in authorIds)
        {
            var user = await _identityService.GetUserByIdAsync(authorId, cancellationToken);
            authors[authorId] = new AuthorDto
            {
                Id = authorId,
                AuthorType = savedPosts.First(s => s.Post.AuthorId == authorId).Post.AuthorType,
                FullName = user?.FullName ?? "Unknown",
                AvatarUrl = user?.AvatarUrl
            };
        }

        var items = savedPosts.Select(s => new SavedPostDto
        {
            Id = s.Id,
            CollectionName = s.CollectionName,
            SavedAt = s.CreatedAt,
            Post = new PostFeedDto
            {
                Id = s.Post.Id,
                Author = authors.GetValueOrDefault(s.Post.AuthorId) ?? new AuthorDto { Id = s.Post.AuthorId, FullName = "Unknown" },
                Content = s.Post.Content,
                Category = s.Post.Category,
                IsRepost = s.Post.IsRepost,
                RepostComment = s.Post.RepostComment,
                OriginalPostId = s.Post.OriginalPostId,
                ReactionCount = s.Post.ReactionCount,
                CommentCount = s.Post.CommentCount,
                RepostCount = s.Post.RepostCount,
                ViewCount = s.Post.ViewCount,
                AllowComments = s.Post.AllowComments,
                IsInternalCase = s.Post.IsInternalCase,
                ConsultationSessionId = s.Post.ConsultationSessionId,
                AiScreeningId = s.Post.AuthorId == request.UserId
                    ? s.Post.AiScreeningId
                    : null,
                PatientAge = s.Post.PatientAge,
                PatientGender = s.Post.PatientGender,
                Attachments = s.Post.Attachments.Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    Type = a.Type,
                    FileName = a.FileName,
                    FileUrl = a.FileUrl,
                    MimeType = a.MimeType,
                    FileSize = a.FileSize,
                    DisplayOrder = a.DisplayOrder
                }).OrderBy(a => a.DisplayOrder).ToList(),
                IsBookmarked = true,
                CreatedAt = s.Post.CreatedAt
            }
        }).ToList();

        var pagedResult = new PagedResult<SavedPostDto>(
            items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<SavedPostDto>>.Success(pagedResult);
    }
}
