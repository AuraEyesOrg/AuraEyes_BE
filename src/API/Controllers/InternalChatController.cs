using Application.Common.Constants;
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

    public InternalChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Search for internal system users (Staff/Doctors) to add to groups.
    /// </summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(ApiResponse<List<UserAdminDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] string? searchTerm)
    {
        var result = await _mediator.Send(new Application.Network.InternalChat.Queries.GetInternalUsers.GetInternalUsersQuery { SearchTerm = searchTerm });
        return Ok(ApiResponseFactory.Success(result));
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
}

public class CreateMeetingRequest
{
    public string Title { get; set; } = string.Empty;
}

public class SendMessageGrRequest
{
    public string Content { get; set; } = string.Empty;
}
