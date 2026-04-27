using Application.ClinicScreenings.Queries.GetClinicScreeningHistory;
using Application.Common.Models;
using Application.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ClinicScreeningHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory([FromQuery] int take = 50, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetClinicScreeningHistoryQuery { Take = take }, cancellationToken);
        return HandleResult(result, "Clinic screening history loaded");
    }
}
