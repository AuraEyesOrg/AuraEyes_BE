using Application.Common.Models;
using Application.Scheduling.Appointments.Commands.CheckInClinicAppointment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/visits")]
public class VisitsController : BaseApiController
{
    private readonly IMediator _mediator;

    public VisitsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Step 1: Check-in patient for a visit/appointment.
    /// Creates a MedicalRecord automatically.
    /// </summary>
    [HttpPost("{id:guid}/check-in")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckIn(Guid id)
    {
        // Re-using the same command logic for consistency
        var result = await _mediator.Send(new CheckInClinicAppointmentCommand(id));
        return HandleResult(result, "Patient checked in successfully.");
    }
}
