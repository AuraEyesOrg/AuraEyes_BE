using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
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
    private readonly IOphthalmologistScreeningReadService _readService;
    private readonly ICurrentUserService _currentUser;

    public OphthalmologistScreeningsController(
        IOphthalmologistScreeningReadService readService,
        ICurrentUserService currentUser)
    {
        _readService = readService;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Application.OphthalmologistScreenings.OphthalmologistScreeningListItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        if (_currentUser.ProfileId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("Unable to resolve ophthalmologist profile."));

        var items = await _readService.ListForOphthalmologistAsync(_currentUser.ProfileId.Value, cancellationToken);
        return Ok(ApiResponseFactory.Success(items, "Screenings loaded"));
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

        var detail = await _readService.GetDetailForOphthalmologistAsync(
            _currentUser.ProfileId.Value,
            screeningId,
            cancellationToken);

        if (detail is null)
            return NotFound(ApiResponseFactory.NotFound("Screening not found or you do not have access."));

        return Ok(ApiResponseFactory.Success(detail, "Screening detail loaded"));
    }
}
