using Application.Common.Constants;
using Application.Common.Models;
using Application.Scheduling.Appointments.Commands.CancelClinicAppointment;
using Application.Scheduling.Appointments.Commands.CreateClinicAppointment;
using Application.Scheduling.Appointments.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Identity.Authorization;

namespace API.Controllers;

[Route("api/clinic-appointments")]
public class ClinicAppointmentsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ClinicAppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AuthorizePermission(Permissions.AppointmentsCreate)]
    [ProducesResponseType(typeof(ApiResponse<CreateClinicAppointmentResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateClinicAppointment([FromBody] CreateClinicAppointmentRequest request)
    {
        var command = new CreateClinicAppointmentCommand
        {
            SlotId = request.SlotId,
            VisitReason = request.VisitReason
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{appointmentId:guid}")]
    [AuthorizePermission(Permissions.AppointmentsManage)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelClinicAppointment(
        Guid appointmentId,
        [FromBody] CancelClinicAppointmentRequest? request = null)
    {
        var command = new CancelClinicAppointmentCommand
        {
            AppointmentId = appointmentId,
            Reason = request?.Reason
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

}

public record CreateClinicAppointmentRequest
{
    public Guid SlotId { get; init; }
    public string? VisitReason { get; init; }
}

public record CancelClinicAppointmentRequest
{
    public string? Reason { get; init; }
}

