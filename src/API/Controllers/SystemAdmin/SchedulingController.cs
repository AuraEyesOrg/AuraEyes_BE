using Application.Common.Constants;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Commands.TriggerSlotGeneration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// Administrative scheduling endpoints for clinic owners.
/// </summary>
[Route("api/system-admin/scheduling")]
[Authorize(Policy = Policies.SystemAdminOnly)]
public class SchedulingController : BaseApiController
{
    private readonly IMediator _mediator;

    public SchedulingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Manually trigger the recurring slot generation job.
    /// This will generate missing slots for the next configured window (e.g., 14 days).
    /// </summary>
    [HttpPost("trigger-generation")]
    [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TriggerSlotGeneration()
    {
        var result = await _mediator.Send(new TriggerSlotGenerationCommand());
        return HandleResult(result);
    }
}
