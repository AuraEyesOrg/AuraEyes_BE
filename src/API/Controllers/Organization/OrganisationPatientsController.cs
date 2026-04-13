using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.OrganisationPatients;
using Application.OrganisationPatients.Commands.UpdateOrganisationPatientContact;
using Application.OrganisationPatients.Queries.GetOrganisationRecentPatients;
using Application.OrganisationPatients.Commands.CreateWalkInPatient;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Organization;

[Route("api/organisations/patients")]
[Authorize(Policy = Policies.OrgAdminOnly)]
public class OrganisationPatientsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public OrganisationPatientsController(
        IMediator mediator,
        ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrganisationRecentPatientDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRecentPatients(
        [FromQuery] int take = 20,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("Unable to resolve current user."));

        var result = await _mediator.Send(
            new GetOrganisationRecentPatientsQuery(_currentUser.UserId.Value, take),
            cancellationToken);

        return HandleResult(result, "Recent patients loaded");
    }

    [HttpPost("walk-in")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWalkInPatient(
        [FromBody] CreateWalkInPatientCommand command,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("Unable to resolve current user."));

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Walk-in patient created successfully");
    }

    [HttpPut("{patientId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePatient(
        Guid patientId,
        [FromBody] UpdateOrganisationPatientRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("Unable to resolve current user."));

        var command = new UpdateOrganisationPatientContactCommand
        {
            OrgAdminUserId = _currentUser.UserId.Value,
            PatientId = patientId,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            CitizenId = request.CitizenId,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            Bmi = request.Bmi,
            DiseaseHistory = request.DiseaseHistory,
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Patient updated successfully");
    }
}

public sealed record UpdateOrganisationPatientRequest
{
    // Walk-in only fields (ignored by backend for registered patients)
    public string? FullName { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string? CitizenId { get; init; }

    // Shared fields
    public string? Address { get; init; }
    public string? PhoneNumber { get; init; }
    public decimal? Bmi { get; init; }
    public string? DiseaseHistory { get; init; }
}
