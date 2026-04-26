using Application.Common.Models;
using Application.Users.Commands.ForceUpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// User management endpoints for non-admin operations (Profile updates, etc.)
/// </summary>
[Route("api/[controller]")]
[Authorize]
public class UsersController : BaseApiController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Force update profile for new users (First Login flow).
    /// Updates basic info, role-specific info, and changes temporary password.
    /// </summary>
    [HttpPut("force-update-profile")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ForceUpdateProfile([FromBody] ForceUpdateProfileCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result, "Profile updated and password changed successfully.");
    }
}
