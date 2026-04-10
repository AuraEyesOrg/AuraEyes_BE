using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.Ophthalmologists.Commands.BackfillFullTimeSchedule;
using Application.SystemAdmin.Ophthalmologists.Commands.DeleteFutureOphthalmologistSlots;
using Application.SystemAdmin.Ophthalmologists.Commands.NormalizeAllFullTimeSchedules;
using Application.SystemAdmin.Ophthalmologists.Commands.NormalizeFullTimeSchedule;
using Application.SystemAdmin.Ophthalmologists.Commands.VerifyOphthalmologist;
using Application.SystemAdmin.Ophthalmologists.Queries.GetOphthalmologists;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// System Admin Ophthalmologist Management endpoints.
/// Credential verification, search, and listing.
/// </summary>
[Route("api/system-admin/[controller]")]
[Authorize(Policy = Policies.SystemAdminOnly)]
public class OphthalmologistsController : BaseApiController
{
    private readonly IMediator _mediator;

    public OphthalmologistsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get ophthalmologists with pagination and filtering
    /// </summary>
    /// <param name="searchTerm">Search by name, email, or phone</param>
    /// <param name="verificationStatus">Filter: PendingVerification, Approved, Rejected</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OphthalmologistListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOphthalmologists(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? verificationStatus = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetOphthalmologistsQuery
        {
            SearchTerm = searchTerm,
            VerificationStatus = verificationStatus,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Approve or reject ophthalmologist credential verification.
    /// </summary>
    /// <param name="id">Ophthalmologist ID</param>
    /// <param name="request">Verification decision</param>
    [HttpPut("{id:guid}/verify")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyOphthalmologist(
        Guid id,
        [FromBody] VerifyOphthalmologistRequest request)
    {
        var command = new VerifyOphthalmologistCommand
        {
            OphthalmologistId = id,
            Approve = request.Approve,
            RejectionReason = request.RejectionReason
        };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Force backfill system-generated full-time templates and slots for one ophthalmologist.
    /// Useful for recovery/testing when old scheduler failures caused missing data.
    /// </summary>
    /// <param name="id">Ophthalmologist ID</param>
    /// <param name="windowDays">Optional rolling window size in days (default from settings/environment)</param>
    [HttpPost("{id:guid}/backfill-fulltime-schedule")]
    [ProducesResponseType(typeof(ApiResponse<BackfillFullTimeScheduleResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> BackfillFullTimeSchedule(Guid id, [FromQuery] int? windowDays = null)
    {
        var result = await _mediator.Send(new BackfillFullTimeScheduleCommand
        {
            OphthalmologistId = id,
            WindowDays = windowDays
        });

        return HandleResult(result, "Backfill full-time schedule completed successfully.");
    }

    /// <summary>
    /// Delete all future slots of one ophthalmologist.
    /// Past slots are preserved.
    /// </summary>
    /// <param name="id">Ophthalmologist ID</param>
    [HttpDelete("{id:guid}/future-slots")]
    [ProducesResponseType(typeof(ApiResponse<DeleteFutureOphthalmologistSlotsResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFutureSlots(Guid id)
    {
        var result = await _mediator.Send(new DeleteFutureOphthalmologistSlotsCommand
        {
            OphthalmologistId = id
        });

        return HandleResult(result, "Delete future slots completed successfully.");
    }

    /// <summary>
    /// Normalize one full-time ophthalmologist schedule to canonical system-generated weekday templates.
    /// Deactivates non-canonical templates, cleans future mismatched slots, and creates missing canonical slots.
    /// </summary>
    /// <param name="id">Ophthalmologist ID</param>
    /// <param name="windowDays">Optional reconciliation window in days (default from settings/environment)</param>
    [HttpPost("{id:guid}/normalize-fulltime-schedule")]
    [ProducesResponseType(typeof(ApiResponse<NormalizeFullTimeScheduleResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> NormalizeFullTimeSchedule(Guid id, [FromQuery] int? windowDays = null)
    {
        var result = await _mediator.Send(new NormalizeFullTimeScheduleCommand
        {
            OphthalmologistId = id,
            WindowDays = windowDays
        });

        return HandleResult(result, "Normalize full-time schedule completed successfully.");
    }

    /// <summary>
    /// Bulk normalize all full-time ophthalmologist schedules to canonical weekday templates.
    /// Default reconciliation window is 7 days when not provided.
    /// </summary>
    /// <param name="windowDays">Optional reconciliation window in days (default: 7)</param>
    [HttpPost("normalize-fulltime-schedules/bulk")]
    [ProducesResponseType(typeof(ApiResponse<NormalizeAllFullTimeSchedulesResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> NormalizeAllFullTimeSchedules([FromQuery] int? windowDays = null)
    {
        var result = await _mediator.Send(new NormalizeAllFullTimeSchedulesCommand
        {
            WindowDays = windowDays
        });

        return HandleResult(result, "Bulk normalize full-time schedules completed successfully.");
    }
}

/// <summary>
/// Request body for ophthalmologist verification.
/// </summary>
public class VerifyOphthalmologistRequest
{
    public bool Approve { get; set; }
    public string? RejectionReason { get; set; }
}
