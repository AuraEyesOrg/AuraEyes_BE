using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.OrganisationPatients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/organisations/patients")]
[Authorize(Policy = Policies.OrgAdminOnly)]
public class OrganisationPatientsController : BaseApiController
{
    private readonly IOrganisationRecentPatientsReadService _readService;
    private readonly ICurrentUserService _currentUser;

    public OrganisationPatientsController(
        IOrganisationRecentPatientsReadService readService,
        ICurrentUserService currentUser)
    {
        _readService = readService;
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

        var items = await _readService.GetRecentPatientsForOrganisationAdminAsync(
            _currentUser.UserId.Value,
            take,
            cancellationToken);

        return Ok(ApiResponseFactory.Success(items, "Recent patients loaded"));
    }
}

