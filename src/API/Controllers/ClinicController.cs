using Application.Common.Constants;
using Application.Common.Models;
using Application.Patients.Commands.UpdateClinicPatient;
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

    /// <summary>
    /// Clinic staff update a patient's demographic and/or medical info.
    /// Demographics are applied via IIdentityService; BMI/DiseaseHistory
    /// are stored on the Patient domain entity.
    /// </summary>
    [HttpPut("patients/{patientId:guid}")]
    [AuthorizePermission(Permissions.PatientsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateClinicPatient(
        Guid patientId,
        [FromBody] UpdateClinicPatientRequest request)
    {
        var command = new UpdateClinicPatientCommand
        {
            PatientId = patientId,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            CitizenId = request.CitizenId,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Address = request.Address,
            Bmi = request.Bmi,
            DiseaseHistory = request.DiseaseHistory,
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}

public record UpdateClinicPatientRequest
{
    public string? FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? CitizenId { get; init; }
    public string? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string? Address { get; init; }
    public decimal? Bmi { get; init; }
    public string? DiseaseHistory { get; init; }
}
