using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Patients.Queries.GetDashboardMetrics;
using Application.Scheduling.Appointments.Common;
using Application.Scheduling.Appointments.Queries.GetPatientClinicAppointments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/patients")]
public class PatientsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public PatientsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("dashboard-metrics")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<PatientDashboardMetricsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(new GetDashboardMetricsQuery(_currentUserService.UserId.Value));
        return HandleResult(result);
    }

    [HttpGet("{patientId:guid}/clinic-appointments")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ClinicAppointmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatientClinicAppointments(Guid patientId)
    {
        var result = await _mediator.Send(new GetPatientClinicAppointmentsQuery(patientId));
        return HandleResult(result);
    }
}
