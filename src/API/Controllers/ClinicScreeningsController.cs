using Application.ClinicScreenings.Queries.GetClinicScreeningHistory;
using Application.Common.Models;
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
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Domain.Entities.Users.Patient> _patientRepository;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public ClinicScreeningsController(IMediator mediator)
    {
        _mediator = mediator;
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets the history of all screenings in the clinic.
    /// </summary>
    [HttpGet("history")]
    [AuthorizePermission(Permissions.ScreeningRead)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ClinicScreeningHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory([FromQuery] int take = 50, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetClinicScreeningHistoryQuery { Take = take }, cancellationToken);
        return HandleResult(result, "Clinic screening history loaded");
    }
}
