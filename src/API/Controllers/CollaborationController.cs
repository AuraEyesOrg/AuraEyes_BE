using Application.Common.Constants;
using Application.Common.Models;
using Application.Network.InternalChat.Commands.Consilium;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Clinical Collaboration (Consilium) endpoints.
/// </summary>
[Route("api/collaboration")]
[Authorize(Policy = Policies.Authenticated)]
public class CollaborationController : BaseApiController
{
    private readonly IMediator _mediator;

    public CollaborationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new clinical consilium group chat.
    /// </summary>
    [HttpPost("clinical-group")]
    [Authorize(Roles = Roles.Ophthalmologist)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateClinicalGroup([FromBody] CreateClinicalGroupCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result, "Clinical consilium group created successfully");
    }

    /// <summary>
    /// Conclude a clinical consilium.
    /// </summary>
    [HttpPost("conclude-consilium/{groupId:guid}")]
    [Authorize(Roles = Roles.Ophthalmologist)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConcludeConsilium(Guid groupId)
    {
        var result = await _mediator.Send(new ConcludeConsiliumCommand { GroupId = groupId });
        return HandleResult(result, "Consilium concluded successfully");
    }
}
