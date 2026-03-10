using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Network.Comments.Commands.CreateComment;
using Application.Network.Comments.Commands.DeleteComment;
using Application.Network.Comments.Queries.GetComments;
using Application.Network.Posts.Commands.CreatePost;
using Application.Network.Posts.Commands.DeletePost;
using Application.Network.Posts.Commands.UpdatePost;
using Application.Network.Posts.Queries.GetFeed;
using Application.Network.Posts.Queries.GetPostById;
using Application.Network.Reactions.Commands.ToggleReaction;
using Application.Network.SavedPosts.Commands.ToggleSavePost;
using Application.Network.SavedPosts.Queries.GetSavedPosts;
using Application.Network.Trending.Queries.GetTrendingTopics;
using Domain.Enums.Network;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Professional Network endpoints.
/// Provides feed, posts, comments, reactions, saved posts and trending topics.
/// </summary>
[Route("api/network")]
[Authorize(Policy = Policies.Authenticated)]
public class NetworkController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public NetworkController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    #region Posts

    /// <summary>
    /// Get paginated feed of professional posts.
    /// </summary>
    [HttpGet("feed")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PostFeedDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeed(
        [FromQuery] PostCategory? category = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetFeedQuery
        {
            CurrentUserId = _currentUserService.UserId!.Value,
            Category = category,
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a single post by ID with full details.
    /// </summary>
    [HttpGet("posts/{postId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PostDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPost(Guid postId)
    {
        var query = new GetPostByIdQuery
        {
            PostId = postId,
            CurrentUserId = _currentUserService.UserId!.Value
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new professional post.
    /// </summary>
    [HttpPost("posts")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        var command = new CreatePostCommand
        {
            AuthorId = _currentUserService.UserId!.Value,
            AuthorType = request.AuthorType,
            Content = request.Content,
            Category = request.Category,
            OrganisationId = request.OrganisationId,
            Visibility = request.Visibility,
            AllowComments = request.AllowComments
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Post created successfully");
    }

    /// <summary>
    /// Update an existing post.
    /// </summary>
    [HttpPut("posts/{postId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostRequest request)
    {
        var command = new UpdatePostCommand
        {
            PostId = postId,
            AuthorId = _currentUserService.UserId!.Value,
            Content = request.Content,
            Visibility = request.Visibility,
            AllowComments = request.AllowComments
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Post updated successfully");
    }

    /// <summary>
    /// Delete a post (soft delete).
    /// </summary>
    [HttpDelete("posts/{postId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        var command = new DeletePostCommand
        {
            PostId = postId,
            AuthorId = _currentUserService.UserId!.Value
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Post deleted successfully");
    }

    #endregion

    #region Comments

    /// <summary>
    /// Get comments for a post with pagination.
    /// </summary>
    [HttpGet("posts/{postId:guid}/comments")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CommentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComments(
        Guid postId,
        [FromQuery] Guid? parentCommentId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetCommentsQuery
        {
            PostId = postId,
            ParentCommentId = parentCommentId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a comment on a post.
    /// </summary>
    [HttpPost("posts/{postId:guid}/comments")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateComment(Guid postId, [FromBody] CreateCommentRequest request)
    {
        var command = new CreateCommentCommand
        {
            PostId = postId,
            AuthorId = _currentUserService.UserId!.Value,
            AuthorType = request.AuthorType,
            Content = request.Content,
            ParentCommentId = request.ParentCommentId
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Comment added successfully");
    }

    /// <summary>
    /// Delete a comment (soft delete).
    /// </summary>
    [HttpDelete("comments/{commentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var command = new DeleteCommentCommand
        {
            CommentId = commentId,
            AuthorId = _currentUserService.UserId!.Value
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Comment deleted successfully");
    }

    #endregion

    #region Reactions

    /// <summary>
    /// Toggle a reaction on a post (add/remove/change).
    /// </summary>
    [HttpPost("posts/{postId:guid}/reactions")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleReaction(Guid postId, [FromBody] ToggleReactionRequest request)
    {
        var command = new ToggleReactionCommand
        {
            PostId = postId,
            UserId = _currentUserService.UserId!.Value,
            Type = request.Type
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    #endregion

    #region Saved Posts

    /// <summary>
    /// Get current user's saved posts.
    /// </summary>
    [HttpGet("saved-posts")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<SavedPostDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSavedPosts(
        [FromQuery] string? collectionName = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetSavedPostsQuery
        {
            UserId = _currentUserService.UserId!.Value,
            CollectionName = collectionName,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Toggle save/unsave a post.
    /// </summary>
    [HttpPost("posts/{postId:guid}/save")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleSavePost(Guid postId, [FromBody] ToggleSavePostRequest? request = null)
    {
        var command = new ToggleSavePostCommand
        {
            UserId = _currentUserService.UserId!.Value,
            PostId = postId,
            CollectionName = request?.CollectionName
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    #endregion

    #region Trending

    /// <summary>
    /// Get trending topics in the professional network.
    /// </summary>
    [HttpGet("trending")]
    [ProducesResponseType(typeof(ApiResponse<List<TrendingTopicDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrendingTopics([FromQuery] int count = 5)
    {
        var query = new GetTrendingTopicsQuery { Count = count };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    #endregion
}

#region Request DTOs

public class CreatePostRequest
{
    public AuthorType AuthorType { get; set; }
    public string Content { get; set; } = string.Empty;
    public PostCategory Category { get; set; }
    public Guid? OrganisationId { get; set; }
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    public bool AllowComments { get; set; } = true;
}

public class UpdatePostRequest
{
    public string Content { get; set; } = string.Empty;
    public PostVisibility? Visibility { get; set; }
    public bool? AllowComments { get; set; }
}

public class CreateCommentRequest
{
    public AuthorType AuthorType { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
}

public class ToggleReactionRequest
{
    public ReactionType Type { get; set; }
}

public class ToggleSavePostRequest
{
    public string? CollectionName { get; set; }
}

#endregion
