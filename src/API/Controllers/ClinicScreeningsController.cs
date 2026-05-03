using Application.ClinicScreenings.Queries.GetClinicScreeningHistory;

using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;

using Application.Screenings.Queries.GetClinicScreeningHistory;
using Application.Screenings.Queries.GetScreeningSessionDetail;
using Application.Screenings.Commands.CreateAiScreeningSession;
using Application.Screenings.Commands.CreateClinicScreeningSession;
using Application.Screenings.Queries.ExportPatientScreeningReportPdf;
using Domain.Entities.Screening;
using Domain.Repositories;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity.Authorization;

namespace API.Controllers;

/// <summary>
/// Clinic-wide screening operations for clinic staff.
/// Replaces the old organization-specific screening endpoints.
/// </summary>
[Route("api/clinic-screenings")]
[AuthorizePermission(Permissions.ScreeningRead)]
public class ClinicScreeningsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ClinicScreeningsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets the history of all screenings in the clinic.
    /// </summary>
    [HttpGet("history")]
    [AuthorizePermission(Permissions.ScreeningRead)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Application.ClinicScreenings.Queries.GetClinicScreeningHistory.ClinicScreeningHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory([FromQuery] int take = 50, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new Application.ClinicScreenings.Queries.GetClinicScreeningHistory.GetClinicScreeningHistoryQuery { Take = take }, cancellationToken);
        return HandleResult(result, "Clinic screening history loaded");
    }

    /// <summary>
    /// Create a new screening session for a patient (called by clinic staff).
    /// </summary>
    [HttpPost("create-session")]
    [AuthorizePermission(Permissions.ScreeningCreate)]
    public async Task<IActionResult> CreateSession([FromBody] CreateClinicScreeningSessionCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Screening session created");
    }

    /// <summary>
    /// Gets detail for a specific screening session.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AuthorizePermission(Permissions.ScreeningRead)]
    public async Task<IActionResult> GetDetail(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetScreeningSessionDetailQuery(id) { BypassPatientCheck = true }, cancellationToken);
        return HandleResult(result, "Screening session detail loaded");
    }

    /// <summary>
    /// Export screening report as PDF.
    /// </summary>
    [HttpGet("{id:guid}/report-pdf")]
    [AuthorizePermission(Permissions.ScreeningRead)]
    public async Task<IActionResult> ExportPdf(Guid id, CancellationToken cancellationToken)
    {
        // For clinic staff, we use Guid.Empty for RequesterUserId and set BypassAccessCheck
        var result = await _mediator.Send(new ExportPatientScreeningReportPdfQuery(Guid.Empty, id, null) { BypassAccessCheck = true }, cancellationToken);
        
        if (!result.IsSuccess || result.Data is null)
            return HandleResult(result, "Screening report generated");

        Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");

        return File(
            result.Data.Content,
            result.Data.ContentType,
            result.Data.FileName);
    }
}
