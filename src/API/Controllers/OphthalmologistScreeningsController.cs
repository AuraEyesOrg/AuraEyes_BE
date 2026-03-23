using Application.Common.Constants;
using Application.OphthalmologistScreenings.Queries.GetOphthalmologistScreeningDetail;
using Application.OphthalmologistScreenings.Queries.ListOphthalmologistScreenings;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// AI screenings visible to the logged-in ophthalmologist (via linked consultation sessions).
/// </summary>
[Route("api/ophthalmologist/screenings")]
[Authorize(Policy = Policies.OphthalmologistOnly)]
public class OphthalmologistScreeningsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public OphthalmologistScreeningsController(
        IMediator mediator,
        ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Application.OphthalmologistScreenings.OphthalmologistScreeningListItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        if (_currentUser.ProfileId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("Unable to resolve ophthalmologist profile."));

        var result = await _mediator.Send(
            new ListOphthalmologistScreeningsQuery { OphthalmologistProfileId = _currentUser.ProfileId.Value },
            cancellationToken);

        if (!result.IsSuccess)
            return HandleResult(result, "Screenings loaded");

        return Ok(ApiResponseFactory.Success(result.Data, "Screenings loaded"));
    }

    [HttpGet("{screeningId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<Application.OphthalmologistScreenings.OphthalmologistScreeningDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid screeningId,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.ProfileId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("Unable to resolve ophthalmologist profile."));

        var result = await _mediator.Send(
            new GetOphthalmologistScreeningDetailQuery
            {
                OphthalmologistProfileId = _currentUser.ProfileId.Value,
                ScreeningId = screeningId
            },
            cancellationToken);

        return HandleResult(result, "Screening detail loaded");
    }
}
