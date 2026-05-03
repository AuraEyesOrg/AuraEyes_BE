using Application.Common.Models;
using Application.Users.Commands.ChangePassword;
using Application.Ophthalmologists.Commands.OnboardOphthalmologist;
using Application.ClinicStaffs.Commands.OnboardClinicStaff;
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
    /// Change password for the current user.
    /// </summary>
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result, "Password changed successfully.");
    }

    /// <summary>
    /// Onboard ophthalmologist (First Login flow).
    /// Updates identity, medical credentials, and changes temporary password.
    /// </summary>
    [HttpPost("onboard-ophthalmologist")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> OnboardOphthalmologist([FromForm] OnboardOphthalmologistCommand command)
    {
        Console.WriteLine($"[DEBUG] OnboardOphthalmologist: FullName={command.FullName}, DegreesCount={command.Degrees?.Count}");
        
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Console.WriteLine($"[DEBUG] ModelState Invalid: {string.Join(", ", errors)}");
            return BadRequest(new { Message = "Model state invalid", Errors = errors });
        }

        var result = await _mediator.Send(command);
        return HandleResult(result, "Ophthalmologist profile onboarded successfully.");
    }

    /// <summary>
    /// Onboard clinic staff (First Login flow).
    /// Updates identity and staff details.
    /// </summary>
    [HttpPost("onboard-clinic-staff")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> OnboardClinicStaff([FromBody] OnboardClinicStaffCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result, "Clinic staff profile onboarded successfully.");
    }
}
