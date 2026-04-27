using Application.Common.Constants;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Application.Scheduling.AppointmentSlots.Queries.GetClinicSchedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Identity.Authorization;

namespace API.Controllers;

[Route("api/clinic-schedule")]
public class ClinicScheduleController : BaseApiController
{
    private readonly IMediator _mediator;

    public ClinicScheduleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get the aggregated clinic schedule (slots grouped by time).
    /// Replaces the old /api/organisations/{id}/schedule endpoint.
    /// </summary>
    [HttpGet]
    [AuthorizePermission(Permissions.AppointmentsRead)]
    [ProducesResponseType(typeof(ApiResponse<ClinicScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClinicSchedule(
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        var result = await _mediator.Send(new GetClinicScheduleQuery 
        { 
            FromDate = fromDate, 
            ToDate = toDate 
        });
        return HandleResult(result);
    }
}
