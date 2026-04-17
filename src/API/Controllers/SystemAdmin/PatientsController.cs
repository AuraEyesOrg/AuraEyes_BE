using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.Patients.Queries.GetPatientMetrics;
using Application.SystemAdmin.Patients.Queries.GetPatients;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Identity.Authorization;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// System Admin Patient Management endpoints.
/// Paginated list, search, lock/unlock.
/// </summary>
[Route("api/system-admin/[controller]")]
[AuthorizePermission(Permissions.PatientsRead)]
public class PatientsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;

    public PatientsController(IMediator mediator, UserManager<ApplicationUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    [HttpGet("metrics")]
    [ProducesResponseType(typeof(ApiResponse<PatientMetricsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatientMetrics()
    {
        var result = await _mediator.Send(new GetPatientMetricsQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Get patients with pagination and filtering
    /// </summary>
    /// <param name="searchTerm">Search by name or email</param>
    /// <param name="status">Filter: Active, Pending, Suspended</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatients(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetPatientsQuery
        {
            SearchTerm = searchTerm,
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Lock or unlock a patient account.
    /// </summary>
    /// <param name="userId">The ApplicationUser ID of the patient</param>
    /// <param name="request">Lock/unlock action</param>
    [HttpPatch("{userId:guid}/status")]
    [AuthorizePermission(Permissions.PatientsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePatientStatus(Guid userId, [FromBody] PatientStatusRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
        {
            return NotFound(ApiResponseFactory.NotFound("Patient not found"));
        }

        switch (request.Action.ToLower())
        {
            case "activate":
                user.IsActive = true;
                break;
            case "suspend":
            case "lock":
                user.IsActive = false;
                break;
            default:
                return BadRequest(ApiResponseFactory.Error($"Unknown action: {request.Action}"));
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Ok(ApiResponseFactory.Success(new { success = true }, $"Patient {request.Action} successfully"));
    }
}

public class PatientStatusRequest
{
    public string Action { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
