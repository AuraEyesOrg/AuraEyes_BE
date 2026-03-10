using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Network.Common;
using Domain.Repositories;

namespace Application.Network.Comments.Queries.GetComments;

/// <summary>
/// Handler for GetCommentsQuery
/// </summary>
public class GetCommentsQueryHandler : IQueryHandler<GetCommentsQuery, PagedResult<CommentDto>>
{
    private readonly IPostRepository _postRepository;
    private readonly IIdentityService _identityService;

    public GetCommentsQueryHandler(IPostRepository postRepository, IIdentityService identityService)
    {
        _postRepository = postRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<CommentDto>>> Handle(
        GetCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var (comments, totalCount) = await _postRepository.GetCommentsAsync(
            request.PostId,
            request.ParentCommentId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        // Build author info
        var authorIds = comments.Select(c => c.AuthorId).Distinct().ToList();
        var authors = new Dictionary<Guid, AuthorDto>();
        foreach (var authorId in authorIds)
        {
            var user = await _identityService.GetUserByIdAsync(authorId, cancellationToken);
            authors[authorId] = new AuthorDto
            {
                Id = authorId,
                AuthorType = comments.First(c => c.AuthorId == authorId).AuthorType,
                FullName = user?.FullName ?? "Unknown",
                AvatarUrl = null
            };
        }

        var items = comments.Select(c => new CommentDto
        {
            Id = c.Id,
            PostId = c.PostId,
            Author = authors.GetValueOrDefault(c.AuthorId) ?? new AuthorDto { Id = c.AuthorId, FullName = "Unknown" },
            Content = c.Content,
            ParentCommentId = c.ParentCommentId,
            ReplyCount = c.ReplyCount,
            LikeCount = c.LikeCount,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        var pagedResult = new PagedResult<CommentDto>(
            items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<CommentDto>>.Success(pagedResult);
    }
}
