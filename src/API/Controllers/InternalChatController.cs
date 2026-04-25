using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Network.InternalChat.Commands.CreateGroupChat;
using Application.Network.InternalChat.Commands.SendMessage;
using Application.Network.InternalChat.Queries.GetGroups;
using Application.Network.InternalChat.Queries.GetMessages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Internal Collaboration Chat endpoints.
/// Provides group chat management and messaging for clinic staff and ophthalmologists.
/// </summary>
[Route("api/internal-chat")]
[Authorize(Policy = Policies.Authenticated)]
public class InternalChatController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<InternalChatController> _logger;

    public InternalChatController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        ILogger<InternalChatController> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    /// <summary>
    /// Get all internal group chats current user belongs to.
    /// </summary>
    [HttpGet("groups")]
    [ProducesResponseType(typeof(ApiResponse<List<InternalGroupChatDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGroups()
    {
        var result = await _mediator.Send(new GetInternalGroupChatsQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new internal group chat.
    /// </summary>
    [HttpPost("groups")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateInternalGroupChatCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result, "Group chat created successfully");
    }

    /// <summary>
    /// Get messages for a specific group chat.
    /// </summary>
    [HttpGet("groups/{groupId:guid}/messages")]
    [ProducesResponseType(typeof(ApiResponse<List<InternalGroupMessageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessages(Guid groupId)
    {
        var result = await _mediator.Send(new GetInternalGroupMessagesQuery { GroupId = groupId });
        return HandleResult(result);
    }

    /// <summary>
    /// Send a message to a group chat.
    /// </summary>
    [HttpPost("groups/{groupId:guid}/messages")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendMessage(Guid groupId, [FromBody] SendMessageGrRequest request)
    {
        var command = new SendInternalGroupMessageCommand
        {
            GroupId = groupId,
            Content = request.Content
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Message sent successfully");
    }

    /// <summary>
    /// Upload internal group chat images.
    /// </summary>
    [HttpPost("groups/{groupId:guid}/upload-images")]
    [ProducesResponseType(typeof(ApiResponse<UploadInternalChatImagesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadGroupImages(
        Guid groupId,
        [FromForm] List<IFormFile> images,
        CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var groupCheckResult = await _mediator.Send(new GetInternalGroupChatsQuery(), cancellationToken);
        if (!groupCheckResult.IsSuccess || groupCheckResult.Data is null)
            return HandleResult(groupCheckResult);

        var isMember = groupCheckResult.Data.Any(g => g.Id == groupId);
        if (!isMember)
            return Forbid();

        if (images is null || images.Count == 0)
            return BadRequest(ApiResponseFactory.Error("No images provided"));

        if (images.Count > 10)
            return BadRequest(ApiResponseFactory.Error("Maximum 10 images allowed"));

        var allowedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/jpg",
            "image/png",
            "image/bmp",
            "image/tiff",
            "image/x-tiff",
            "image/webp"
        };

        var uploadedUrls = new List<string>();

        try
        {
            foreach (var image in images)
            {
                if (image.Length == 0)
                    return BadRequest(ApiResponseFactory.Error($"File '{image.FileName}' is empty"));

                if (image.Length > 50 * 1024 * 1024)
                    return BadRequest(ApiResponseFactory.Error($"File '{image.FileName}' exceeds 50MB limit"));

                if (!allowedTypes.Contains(image.ContentType ?? string.Empty))
                    return BadRequest(ApiResponseFactory.Error(
                        $"File '{image.FileName}' has unsupported format. Only JPG, JPEG, PNG, BMP, TIFF, and WebP are allowed"));

                await using var stream = image.OpenReadStream();
                var uploadedUrl = await _fileStorageService.SaveFileAsync(
                    stream,
                    image.FileName,
                    $"internal-chat/group-images/{groupId}/{_currentUserService.UserId}",
                    cancellationToken);

                uploadedUrls.Add(uploadedUrl);
                _logger.LogInformation("Uploaded internal chat group image to storage: {Url}", uploadedUrl);
            }

            return Ok(ApiResponseFactory.Success(
                new UploadInternalChatImagesResponse
                {
                    UploadedUrls = uploadedUrls,
                    Count = uploadedUrls.Count
                },
                "Internal chat images uploaded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload internal chat images for group {GroupId}", groupId);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponseFactory.Error($"Failed to upload images: {ex.Message}"));
        }
    }

    /// <summary>
    /// Create a Google Meet consultation for the group.
    /// </summary>
    [HttpPost("groups/{groupId:guid}/meetings")]
    [ProducesResponseType(typeof(ApiResponse<MeetingInfo>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateMeeting(Guid groupId, [FromBody] CreateMeetingRequest request)
    {
        var command = new Application.Network.InternalChat.Commands.CreateMeeting.CreateGroupMeetingCommand
        {
            GroupId = groupId,
            Title = request.Title
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Meeting created successfully");
    }

    /// <summary>
    /// Rename an internal group chat.
    /// </summary>
    [HttpPut("groups/{groupId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RenameGroup(Guid groupId, [FromBody] RenameGroupRequest request)
    {
        var command = new Application.Network.InternalChat.Commands.UpdateGroupChat.UpdateInternalGroupChatCommand
        {
            GroupId = groupId,
            Name = request.Name
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Group renamed successfully");
    }

    /// <summary>
    /// Delete an internal group chat.
    /// </summary>
    [HttpDelete("groups/{groupId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteGroup(Guid groupId)
    {
        var command = new Application.Network.InternalChat.Commands.DeleteGroupChat.DeleteInternalGroupChatCommand
        {
            GroupId = groupId
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Group deleted successfully");
    }

    /// <summary>
    /// Update members of an internal group chat.
    /// </summary>
    [HttpPut("groups/{groupId:guid}/members")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateMembers(Guid groupId, [FromBody] UpdateMembersRequest request)
    {
        var command = new Application.Network.InternalChat.Commands.UpdateGroupMembers.UpdateInternalGroupMembersCommand
        {
            GroupId = groupId,
            MemberIds = request.MemberIds
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Group members updated successfully");
    }
}

public class RenameGroupRequest
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateMembersRequest
{
    public List<Guid> MemberIds { get; set; } = new();
}

public class CreateMeetingRequest
{
    public string Title { get; set; } = string.Empty;
}

public class SendMessageGrRequest
{
    public string Content { get; set; } = string.Empty;
}

public record UploadInternalChatImagesResponse
{
    public List<string> UploadedUrls { get; init; } = new();
    public int Count { get; init; }
}
