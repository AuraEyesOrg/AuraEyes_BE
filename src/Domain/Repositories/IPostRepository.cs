using Domain.Common;
using Domain.Entities.Network;
using Domain.Enums.Network;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for ProfessionalPost aggregate root.
/// </summary>
public interface IPostRepository : IRepository<ProfessionalPost>
{
    /// <summary>
    /// Get paginated feed of posts with optional filters.
    /// </summary>
    Task<(IReadOnlyList<ProfessionalPost> Items, int TotalCount)> GetFeedAsync(
        PostCategory? category = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get post by ID with attachments included.
    /// </summary>
    Task<ProfessionalPost?> GetByIdWithAttachmentsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get reaction by user for a specific post.
    /// </summary>
    Task<PostReaction?> GetReactionAsync(
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a reaction to a post.
    /// </summary>
    Task AddReactionAsync(PostReaction reaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a reaction from a post.
    /// </summary>
    Task RemoveReactionAsync(PostReaction reaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get comments for a post with pagination.
    /// </summary>
    Task<(IReadOnlyList<PostComment> Items, int TotalCount)> GetCommentsAsync(
        Guid postId,
        Guid? parentCommentId = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a comment by ID.
    /// </summary>
    Task<PostComment?> GetCommentByIdAsync(
        Guid commentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a comment to a post.
    /// </summary>
    Task AddCommentAsync(PostComment comment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get saved post by user and post ID.
    /// </summary>
    Task<SavedPost?> GetSavedPostAsync(
        Guid userId,
        Guid postId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a saved post.
    /// </summary>
    Task AddSavedPostAsync(SavedPost savedPost, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a saved post.
    /// </summary>
    Task RemoveSavedPostAsync(SavedPost savedPost, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get saved posts for a user with pagination.
    /// </summary>
    Task<(IReadOnlyList<SavedPost> Items, int TotalCount)> GetSavedPostsAsync(
        Guid userId,
        string? collectionName = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get post counts grouped by category for trending.
    /// </summary>
    Task<Dictionary<PostCategory, int>> GetPostCountsByCategoryAsync(
        DateTime since,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user has bookmarked a specific post.
    /// </summary>
    Task<bool> IsPostSavedByUserAsync(
        Guid userId,
        Guid postId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user's reactions for a batch of posts (for feed enrichment).
    /// </summary>
    Task<Dictionary<Guid, ReactionType>> GetUserReactionsForPostsAsync(
        Guid userId,
        IEnumerable<Guid> postIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get bookmark status for a batch of posts (for feed enrichment).
    /// </summary>
    Task<HashSet<Guid>> GetUserSavedPostIdsAsync(
        Guid userId,
        IEnumerable<Guid> postIds,
        CancellationToken cancellationToken = default);
}
