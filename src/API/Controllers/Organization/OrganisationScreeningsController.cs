using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.OrganisationScreenings;
using Application.OrganisationScreenings.Commands.CreateOrgScreeningSession;
using Application.OrganisationScreenings.Queries.GetOrgScreeningSessionDetail;
using Application.OrganisationScreenings.Queries.GetOrgScreeningHistory;
using Application.Screenings.Queries.GetScreeningSessionDetail;
using Application.Screenings.Commands.CreateAiScreeningSession;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Organization;

/// <summary>
/// Organisation AI Screening endpoints.
/// Allows OrgAdmin to perform screenings on behalf of patients.
/// </summary>
[Route("api/organisations/screenings")]
[Authorize(Policy = Policies.OrgAdminOnly)]
public class OrganisationScreeningsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public OrganisationScreeningsController(
        IMediator mediator,
        ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Create an AI screening session on behalf of a patient.
    /// </summary>
    [HttpPost("create-session")]
    [ProducesResponseType(typeof(ApiResponse<CreateOrgScreeningSessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateScreeningSession(
        [FromBody] CreateOrgScreeningRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var command = new CreateOrgScreeningSessionCommand
        {
            PatientId = request.PatientId,
            ModelVersion = request.ModelVersion ?? "CFP_v1",
            RetinalImages = request.RetinalImages ?? new List<RetinalImageData>()
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Organisation screening session created successfully");
    }

    /// <summary>
    /// Get a screening session detail for this organisation.
    /// </summary>
    [HttpGet("{screeningId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ScreeningSessionDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSessionById(
        [FromRoute] Guid screeningId,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var result = await _mediator.Send(
            new GetOrgScreeningSessionDetailQuery(_currentUser.UserId.Value, screeningId),
            cancellationToken);

        return HandleResult(result, "Screening session loaded");
    }

    /// <summary>
    /// Get screening history for this organisation.
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrgScreeningHistoryItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScreeningHistory(
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var result = await _mediator.Send(
            new GetOrgScreeningHistoryQuery
            {
                OrgAdminUserId = _currentUser.UserId.Value,
                Take = take
            },
            cancellationToken);

        return HandleResult(result, "Screening history loaded");
    }
}

public record CreateOrgScreeningRequest
{
    /// <summary>Patient to screen on behalf of.</summary>
    public Guid PatientId { get; init; }

    /// <summary>AI model version.</summary>
    public string? ModelVersion { get; init; }

    /// <summary>Retinal images with URLs already uploaded.</summary>
    public List<RetinalImageData>? RetinalImages { get; init; }
}
