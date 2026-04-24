using Application.ClinicScreenings.Commands.CreateClinicScreeningSession;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.OrganisationScreenings;
using Application.OrganisationScreenings.Commands.ShareOrgScreeningResult;
using Application.OrganisationScreenings.Queries.ExportOrgScreeningReportPdf;
using Application.OrganisationScreenings.Queries.GetOrgScreeningHistory;
using Application.OrganisationScreenings.Queries.GetOrgScreeningSessionDetail;
using Application.Screenings.Commands.CreateAiScreeningSession;
using Application.Screenings.Queries.GetScreeningSessionDetail;
using Infrastructure.Identity.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/clinic-screenings")]
[AuthorizePermission(Permissions.ScreeningRead)]
public class ClinicScreeningsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ClinicScreeningsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpPost("create-session")]
    [AuthorizePermission(Permissions.ScreeningCreate)]
    [ProducesResponseType(typeof(ApiResponse<CreateClinicScreeningSessionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateScreeningSession(
        [FromBody] CreateClinicScreeningRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var command = new CreateClinicScreeningSessionCommand
        {
            PatientId = request.PatientId,
            ModelVersion = request.ModelVersion ?? "AURA_v1.0",
            RetinalImages = request.RetinalImages ?? new List<RetinalImageData>()
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Clinic screening session created successfully");
    }

    [HttpGet("{screeningId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ScreeningSessionDetailDto>), StatusCodes.Status200OK)]
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

    [HttpPost("{screeningId:guid}/share")]
    [AuthorizePermission(Permissions.ScreeningCreate)]
    [ProducesResponseType(typeof(ApiResponse<ShareOrgScreeningResultResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ShareScreeningResult(
        [FromRoute] Guid screeningId,
        [FromBody] ShareClinicScreeningResultRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var command = new ShareOrgScreeningResultCommand
        {
            OrgAdminUserId = _currentUser.UserId.Value,
            ScreeningId = screeningId,
            RecipientEmail = request.RecipientEmail,
            IncludePdf = request.IncludePdf,
            IncludeRetinalImages = request.IncludeRetinalImages
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Screening result shared successfully");
    }

    [HttpGet("{screeningId:guid}/report-pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadScreeningReportPdf(
        [FromRoute] Guid screeningId,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var result = await _mediator.Send(
            new ExportOrgScreeningReportPdfQuery(_currentUser.UserId.Value, screeningId),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
            return HandleResult(result, "Screening report generated");

        Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");

        return File(
            result.Data.Content,
            result.Data.ContentType,
            result.Data.FileName);
    }
}

public record CreateClinicScreeningRequest
{
    public Guid PatientId { get; init; }
    public string? ModelVersion { get; init; }
    public List<RetinalImageData>? RetinalImages { get; init; }
}

public record ShareClinicScreeningResultRequest
{
    public string? RecipientEmail { get; init; }
    public bool IncludePdf { get; init; } = true;
    public bool IncludeRetinalImages { get; init; }
}
