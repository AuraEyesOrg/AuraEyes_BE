using Application.Common.Constants;
using Application.Common.Models;
using Application.Scheduling.Appointments.Commands.CancelClinicAppointment;
using Application.Scheduling.Appointments.Commands.CheckInClinicAppointment;
using Application.Scheduling.Appointments.Commands.CompleteClinicAppointment;
using Application.Scheduling.Appointments.Commands.CreateClinicAppointment;
using Application.Scheduling.Appointments.Commands.MarkClinicAppointmentNoShow;
using Application.Scheduling.Appointments.Commands.StartClinicAppointment;
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
            OrganisationId = request.OrganisationId,
            SlotId = request.SlotId,
            VisitReason = request.VisitReason
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{appointmentId:guid}")]
    [AuthorizePermission(Permissions.AppointmentsUpdate)]
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

    [HttpPut("{appointmentId:guid}/check-in")]
    [AuthorizePermission(Permissions.AppointmentsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckInClinicAppointment(Guid appointmentId)
    {
        var result = await _mediator.Send(new CheckInClinicAppointmentCommand(appointmentId));
        return HandleResult(result);
    }

    [HttpPut("{appointmentId:guid}/start")]
    [AuthorizePermission(Permissions.AppointmentsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartClinicAppointment(Guid appointmentId)
    {
        var result = await _mediator.Send(new StartClinicAppointmentCommand(appointmentId));
        return HandleResult(result);
    }

    [HttpPut("{appointmentId:guid}/complete")]
    [AuthorizePermission(Permissions.AppointmentsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CompleteClinicAppointment(
        Guid appointmentId,
        [FromBody] CompleteClinicAppointmentRequest? request = null)
    {
        var command = new CompleteClinicAppointmentCommand
        {
            AppointmentId = appointmentId,
            Notes = request?.Notes
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{appointmentId:guid}/no-show")]
    [AuthorizePermission(Permissions.AppointmentsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkClinicAppointmentNoShow(Guid appointmentId)
    {
        var result = await _mediator.Send(new MarkClinicAppointmentNoShowCommand(appointmentId));
        return HandleResult(result);
    }
}

public record CreateClinicAppointmentRequest
{
    public Guid OrganisationId { get; init; }
    public Guid SlotId { get; init; }
    public string? VisitReason { get; init; }
}

public record CancelClinicAppointmentRequest
{
    public string? Reason { get; init; }
}

public record CompleteClinicAppointmentRequest
{
    public string? Notes { get; init; }
}
