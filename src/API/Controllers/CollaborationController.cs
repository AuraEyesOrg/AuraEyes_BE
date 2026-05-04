using Application.Common.Constants;
using Application.Common.Models;
using Application.Network.InternalChat.Commands.Consilium;
using Application.Scheduling.Appointments.Common;
using Application.Scheduling.Appointments.Queries.GetAvailableDoctorsForConsilium;
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
    /// Get list of ophthalmologists currently available for a Hot Consilium session.
    /// Availability is derived dynamically from appointment slots and approved leave requests
    /// within the next 25-minute window. Zero schema changes required.
    /// </summary>
    [HttpGet("available-doctors")]
    [Authorize(Roles = Roles.Ophthalmologist)]
    [ProducesResponseType(typeof(ApiResponse<List<AvailableDoctorDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableDoctors()
    {
        var result = await _mediator.Send(new GetAvailableDoctorsForConsiliumQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new clinical consilium group chat.
    /// Automatically injects a system message containing a deep-link to the Medical Record.
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
