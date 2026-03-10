using Domain.Entities.Network;
using Domain.Enums.Network;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ProfessionalPost aggregate root.
/// </summary>
public class PostRepository : Repository<ProfessionalPost>, IPostRepository
{
    public PostRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<ProfessionalPost> Items, int TotalCount)> GetFeedAsync(
        PostCategory? category = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(p => p.Attachments)
            .Where(p => p.Visibility == PostVisibility.Public)
            .AsQueryable();

        if (category.HasValue)
            query = query.Where(p => p.Category == category.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(p => p.Content.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<ProfessionalPost?> GetByIdWithAttachmentsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Attachments)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PostReaction?> GetReactionAsync(
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PostReactions
            .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId, cancellationToken);
    }

    public async Task AddReactionAsync(PostReaction reaction, CancellationToken cancellationToken = default)
    {
        await _context.PostReactions.AddAsync(reaction, cancellationToken);
    }

    public Task RemoveReactionAsync(PostReaction reaction, CancellationToken cancellationToken = default)
    {
        _context.PostReactions.Remove(reaction);
        return Task.CompletedTask;
    }

    public async Task<(IReadOnlyList<PostComment> Items, int TotalCount)> GetCommentsAsync(
        Guid postId,
        Guid? parentCommentId = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.PostComments
            .AsNoTracking()
            .Where(c => c.PostId == postId && c.ParentCommentId == parentCommentId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<PostComment?> GetCommentByIdAsync(
        Guid commentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PostComments
            .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);
    }

    public async Task AddCommentAsync(PostComment comment, CancellationToken cancellationToken = default)
    {
        await _context.PostComments.AddAsync(comment, cancellationToken);
    }

    public async Task<SavedPost?> GetSavedPostAsync(
        Guid userId,
        Guid postId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SavedPosts
            .FirstOrDefaultAsync(s => s.UserId == userId && s.PostId == postId, cancellationToken);
    }

    public async Task AddSavedPostAsync(SavedPost savedPost, CancellationToken cancellationToken = default)
    {
        await _context.SavedPosts.AddAsync(savedPost, cancellationToken);
    }

    public Task RemoveSavedPostAsync(SavedPost savedPost, CancellationToken cancellationToken = default)
    {
        _context.SavedPosts.Remove(savedPost);
        return Task.CompletedTask;
    }

    public async Task<(IReadOnlyList<SavedPost> Items, int TotalCount)> GetSavedPostsAsync(
        Guid userId,
        string? collectionName = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _context.SavedPosts
            .AsNoTracking()
            .Include(s => s.Post)
                .ThenInclude(p => p.Attachments)
            .Where(s => s.UserId == userId);

        if (!string.IsNullOrWhiteSpace(collectionName))
            query = query.Where(s => s.CollectionName == collectionName);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Dictionary<PostCategory, int>> GetPostCountsByCategoryAsync(
        DateTime since,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.CreatedAt >= since && !p.IsRepost)
            .GroupBy(p => p.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Category, x => x.Count, cancellationToken);
    }

    public async Task<bool> IsPostSavedByUserAsync(
        Guid userId,
        Guid postId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SavedPosts
            .AnyAsync(s => s.UserId == userId && s.PostId == postId, cancellationToken);
    }

    public async Task<Dictionary<Guid, ReactionType>> GetUserReactionsForPostsAsync(
        Guid userId,
        IEnumerable<Guid> postIds,
        CancellationToken cancellationToken = default)
    {
        return await _context.PostReactions
            .AsNoTracking()
            .Where(r => r.UserId == userId && postIds.Contains(r.PostId))
            .ToDictionaryAsync(r => r.PostId, r => r.Type, cancellationToken);
    }

    public async Task<HashSet<Guid>> GetUserSavedPostIdsAsync(
        Guid userId,
        IEnumerable<Guid> postIds,
        CancellationToken cancellationToken = default)
    {
        var savedIds = await _context.SavedPosts
            .AsNoTracking()
            .Where(s => s.UserId == userId && postIds.Contains(s.PostId))
            .Select(s => s.PostId)
            .ToListAsync(cancellationToken);

        return savedIds.ToHashSet();
    }
}
