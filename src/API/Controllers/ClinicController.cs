using Application.Common.Constants;
using Application.Common.Models;
using Application.Patients.Queries.GetRecentClinicPatients;
using Application.Clinic.Queries.GetDashboardMetrics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Identity.Authorization;

namespace API.Controllers;

[Route("api/clinic")]
public class ClinicController : BaseApiController
{
    private readonly IMediator _mediator;

    public ClinicController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get recent patients for the clinic staff dashboard.
    /// Replaces the old /api/organisations/patients endpoint.
    /// </summary>
    [HttpGet("patients")]
    [AuthorizePermission(Permissions.PatientsRead)]
    [ProducesResponseType(typeof(ApiResponse<List<RecentClinicPatientDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentPatients()
    {
        var result = await _mediator.Send(new GetRecentClinicPatientsQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Get operational metrics for the clinic staff dashboard.
    /// Replaces the old /api/organisations/dashboard-metrics endpoint.
    /// </summary>
    [HttpGet("dashboard-metrics")]
    [AuthorizePermission(Permissions.DashboardRead)]
    [ProducesResponseType(typeof(ApiResponse<ClinicDashboardMetricsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        var result = await _mediator.Send(new GetClinicDashboardMetricsQuery());
        return HandleResult(result);
    }
}
