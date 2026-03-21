using Application.SystemSettings.Commands.UpdateSystemSettings;
using Application.SystemSettings.Queries.GetSystemSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/system-settings")]
[ApiController]
public class SystemSettingsController : ControllerBase
{
    private readonly ISender _sender;

    public SystemSettingsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets all system settings.
    /// Can be accessed by anyone (used by Patient app to get advance booking limit).
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetSystemSettings(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetSystemSettingsQuery(), cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Updates one or multiple system settings.
    /// Restricted to System Admin role.
    /// </summary>
    [HttpPut]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> UpdateSystemSettings([FromBody] Dictionary<string, string> settings, CancellationToken cancellationToken)
    {
        var command = new UpdateSystemSettingsCommand { Settings = settings };
        var result = await _sender.Send(command, cancellationToken);
        
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
    }
}
