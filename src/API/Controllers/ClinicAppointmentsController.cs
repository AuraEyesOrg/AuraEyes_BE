using Application.Common.Constants;
using Application.Common.Models;
using Application.ClinicAppointments.Commands.AssignDoctor;
using Application.ClinicAppointments.Commands.CancelClinicAppointment;
using Application.ClinicAppointments.Commands.CheckInAppointment;
using Application.ClinicAppointments.Commands.CompleteAppointment;
using Application.ClinicAppointments.Commands.CreateClinicAppointment;
using Application.ClinicAppointments.Commands.MarkNoShow;
using Application.ClinicAppointments.Commands.StartAppointment;
using Application.ClinicAppointments.Common;
using Application.ClinicAppointments.Queries.GetClinicAppointmentDetail;
using Application.ClinicAppointments.Queries.GetOrganisationAppointments;
using Application.ClinicAppointments.Queries.GetOrganisationAvailableSlots;
using Application.ClinicAppointments.Queries.GetPatientClinicAppointments;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

/// <summary>
/// Controller for organisation clinic appointments (patient booking flow).
/// </summary>
[Route("api/clinic-appointments")]
public class ClinicAppointmentsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ClinicAppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Patient Booking

    /// <summary>
    /// Get available slots for an organisation (for patient booking).
    /// </summary>
    [HttpGet("organisations/{organisationId:guid}/available-slots")]
    [AllowAnonymous]  // Public endpoint for patients to browse
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrganisationAvailableSlotDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableSlots(
        Guid organisationId,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        var query = new GetOrganisationAvailableSlotsQuery
        {
            OrganisationId = organisationId,
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a clinic appointment (patient books a slot).
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateClinicAppointmentRequest request)
    {
        var patientId = GetPatientIdFromClaims();
        if (patientId == Guid.Empty)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("Patient ID not found in claims."));
        }

        var command = new CreateClinicAppointmentCommand
        {
            PatientId = patientId,
            OrganisationId = request.OrganisationId,
            SlotId = request.SlotId,
            VisitReason = request.VisitReason
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetAppointmentDetail),
                new { appointmentId = result.Data },
                ApiResponseFactory.Success(result.Data, "Appointment created successfully."));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get patient's clinic appointments.
    /// </summary>
    [HttpGet("patient")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ClinicAppointmentListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatientAppointments(
        [FromQuery] AppointmentStatus? status = null,
        [FromQuery] bool upcomingOnly = false)
    {
        var patientId = GetPatientIdFromClaims();
        if (patientId == Guid.Empty)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("Patient ID not found in claims."));
        }

        var query = new GetPatientClinicAppointmentsQuery
        {
            PatientId = patientId,
            Status = status,
            UpcomingOnly = upcomingOnly
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Cancel an appointment (patient cancellation).
    /// </summary>
    [HttpDelete("{appointmentId:guid}")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelAppointment(Guid appointmentId, [FromBody] CancelAppointmentRequest? request = null)
    {
        var patientId = GetPatientIdFromClaims();
        if (patientId == Guid.Empty)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("Patient ID not found in claims."));
        }

        var command = new CancelClinicAppointmentCommand
        {
            AppointmentId = appointmentId,
            CancelledBy = patientId,
            Reason = request?.Reason
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Appointment cancelled successfully.");
    }

    #endregion

    #region Organisation Staff Operations

    /// <summary>
    /// Get appointments for an organisation (staff view).
    /// </summary>
    [HttpGet("organisations/{organisationId:guid}")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ClinicAppointmentListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganisationAppointments(
        Guid organisationId,
        [FromQuery] DateOnly? date = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] AppointmentStatus? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetOrganisationAppointmentsQuery
        {
            OrganisationId = organisationId,
            Date = date,
            FromDate = fromDate,
            ToDate = toDate,
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get appointment details.
    /// </summary>
    [HttpGet("{appointmentId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ClinicAppointmentDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentDetail(Guid appointmentId)
    {
        var query = new GetClinicAppointmentDetailQuery { AppointmentId = appointmentId };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Check in a patient at the clinic.
    /// </summary>
    [HttpPut("{appointmentId:guid}/check-in")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckIn(Guid appointmentId)
    {
        var command = new CheckInAppointmentCommand { AppointmentId = appointmentId };
        var result = await _mediator.Send(command);
        return HandleResult(result, "Patient checked in successfully.");
    }

    /// <summary>
    /// Assign a doctor to an appointment.
    /// </summary>
    [HttpPut("{appointmentId:guid}/assign-doctor")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignDoctor(Guid appointmentId, [FromBody] AssignDoctorRequest request)
    {
        var command = new AssignDoctorCommand
        {
            AppointmentId = appointmentId,
            DoctorId = request.DoctorId
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Doctor assigned successfully.");
    }

    /// <summary>
    /// Start an appointment (begin consultation).
    /// </summary>
    [HttpPut("{appointmentId:guid}/start")]
    [Authorize(Policy = Policies.MedicalStaff)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartAppointment(Guid appointmentId)
    {
        var command = new StartAppointmentCommand { AppointmentId = appointmentId };
        var result = await _mediator.Send(command);
        return HandleResult(result, "Appointment started.");
    }

    /// <summary>
    /// Complete an appointment.
    /// </summary>
    [HttpPut("{appointmentId:guid}/complete")]
    [Authorize(Policy = Policies.MedicalStaff)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteAppointment(Guid appointmentId, [FromBody] CompleteAppointmentRequest? request = null)
    {
        var command = new CompleteAppointmentCommand
        {
            AppointmentId = appointmentId,
            Notes = request?.Notes
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Appointment completed successfully.");
    }

    /// <summary>
    /// Mark patient as no-show.
    /// </summary>
    [HttpPut("{appointmentId:guid}/no-show")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkNoShow(Guid appointmentId)
    {
        var command = new MarkNoShowCommand { AppointmentId = appointmentId };
        var result = await _mediator.Send(command);
        return HandleResult(result, "Patient marked as no-show.");
    }

    /// <summary>
    /// Cancel an appointment (organisation cancellation).
    /// </summary>
    [HttpPut("{appointmentId:guid}/cancel")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelByOrganisation(Guid appointmentId, [FromBody] CancelAppointmentRequest? request = null)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("User not found."));
        }

        var command = new CancelClinicAppointmentCommand
        {
            AppointmentId = appointmentId,
            CancelledBy = userGuid,
            Reason = request?.Reason
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Appointment cancelled successfully.");
    }

    #endregion

    #region Helpers

    private Guid GetPatientIdFromClaims()
    {
        var patientIdClaim = User.FindFirst("patient_id")?.Value;
        if (string.IsNullOrEmpty(patientIdClaim) || !Guid.TryParse(patientIdClaim, out var patientId))
        {
            return Guid.Empty;
        }
        return patientId;
    }

    #endregion
}

#region Request DTOs

public record CreateClinicAppointmentRequest
{
    public Guid OrganisationId { get; init; }
    public Guid SlotId { get; init; }
    public string? VisitReason { get; init; }
}

public record CancelAppointmentRequest
{
    public string? Reason { get; init; }
}

public record AssignDoctorRequest
{
    public Guid DoctorId { get; init; }
}

public record CompleteAppointmentRequest
{
    public string? Notes { get; init; }
}

#endregion
