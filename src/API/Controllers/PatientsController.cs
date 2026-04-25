using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Patients.Queries.GetDashboardMetrics;
using Application.Scheduling.Appointments.Queries.GetPatientClinicAppointments;
using Application.MedicalRecords.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Identity.Authorization;

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
    [AuthorizePermission(Permissions.DashboardRead)]
    [ProducesResponseType(typeof(ApiResponse<PatientDashboardMetricsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(new GetDashboardMetricsQuery(_currentUserService.UserId.Value));
        return HandleResult(result);
    }

    [HttpGet("{patientId:guid}/clinic-appointments")]
    [AuthorizePermission(Permissions.AppointmentsRead)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ClinicAppointmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatientClinicAppointments(
        Guid patientId,
        [FromQuery] PatientAppointmentTab tab = PatientAppointmentTab.All,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(
            new GetPatientClinicAppointmentsQuery(patientId, tab, pageNumber, pageSize));
        return HandleResult(result);
    }

    [HttpGet("{patientId:guid}/medical-records")]
    // [AuthorizePermission(Permissions.MedicalRecordsRead)]
    [ProducesResponseType(typeof(ApiResponse<List<MedicalRecordDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatientMedicalRecords(Guid patientId)
    {
        var result = await _mediator.Send(new Application.MedicalRecords.Queries.GetPatientMedicalRecords.GetPatientMedicalRecordsQuery(patientId));
        return HandleResult(result);
    }
}
