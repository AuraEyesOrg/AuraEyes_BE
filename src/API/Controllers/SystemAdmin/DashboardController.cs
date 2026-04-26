using Application.Common.Constants;
using Application.Common.Helpers;
using Application.Common.Models;
using Application.SystemAdmin.Dashboard.Queries.GetDashboardMetrics;
using Application.SystemAdmin.Dashboard.Queries.GetDoctorStatus;
using Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkload;
using Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkloads;
using Application.SystemAdmin.Dashboard.Queries.GetLiveQueue;
using Application.SystemAdmin.Dashboard.Queries.GetPartTimeSlotQuotaUsage;
using Application.SystemAdmin.Dashboard.Queries.GetPopulationRiskAnalysis;
using Application.SystemAdmin.Dashboard.Queries.GetRecentScreenings;
using Application.SystemAdmin.Dashboard.Queries.GetScreeningVolumeTrends;
using Application.SystemAdmin.Dashboard.Queries.GetSlotUtilization;
using Application.SystemAdmin.Dashboard.Queries.GetSystemHealth;
using Application.SystemAdmin.Dashboard.Queries.GetTodaySummary;
using Domain.Enums;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Infrastructure.Identity.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// System Admin Dashboard endpoints
/// Provides real-time consolidated overview of system activity, AI screening performance,
/// operational health, and population-level risk insights.
/// </summary>
[Route("api/system-admin/[controller]")]
[AuthorizePermission(Permissions.DashboardRead)]
public class DashboardController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly FullTimeSlotGenerationJob _fullTimeSlotGenerationJob;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IMediator mediator,
        FullTimeSlotGenerationJob fullTimeSlotGenerationJob,
        ILogger<DashboardController> logger)
    {
        _mediator = mediator;
        _fullTimeSlotGenerationJob = fullTimeSlotGenerationJob;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard overview metrics
    /// </summary>
    /// <remarks>
    /// Returns total screenings today, AI accuracy, pending reviews, and quick stats.
    /// Screen: 3.3.1-3.3.4
    /// </remarks>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(ApiResponse<DashboardMetricsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMetrics()
    {
        var result = await _mediator.Send(new GetDashboardMetricsQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Get screening volume trends
    /// </summary>
    /// <param name="timeRange">Time range: "weekly" or "monthly"</param>
    /// <param name="periods">Number of periods to include</param>
    /// <remarks>
    /// Screen: 3.3.5 View Screening Volume Trends
    /// </remarks>
    [HttpGet("screening-trends")]
    [ProducesResponseType(typeof(ApiResponse<ScreeningVolumeTrendsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetScreeningTrends(
        [FromQuery] string timeRange = "monthly",
        [FromQuery] int periods = 12)
    {
        var query = new GetScreeningVolumeTrendsQuery
        {
            TimeRange = timeRange,
            Periods = periods
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get population risk analysis
    /// </summary>
    /// <remarks>
    /// Screen: 3.3.6 View Population Risk Analysis
    /// </remarks>
    [HttpGet("risk-analysis")]
    [ProducesResponseType(typeof(ApiResponse<PopulationRiskAnalysisDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRiskAnalysis()
    {
        var result = await _mediator.Send(new GetPopulationRiskAnalysisQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Get system health status
    /// </summary>
    /// <remarks>
    /// Screen: 3.3.7 View System Health Status
    /// </remarks>
    [HttpGet("system-health")]
    [ProducesResponseType(typeof(ApiResponse<SystemHealthDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSystemHealth()
    {
        var result = await _mediator.Send(new GetSystemHealthQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Get recent screenings list
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <remarks>
    /// Screen: 3.3.8 View Recent Screenings List
    /// </remarks>
    [HttpGet("recent-screenings")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<RecentScreeningDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRecentScreenings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetRecentScreeningsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get part-time slot quota usage by day.
    /// </summary>
    [HttpGet("part-time-slot-usage")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PartTimeSlotQuotaUsageDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPartTimeSlotUsage(
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var query = new GetPartTimeSlotQuotaUsageQuery
        {
            FromDate = fromDate ?? today,
            ToDate = toDate ?? today.AddDays(7)
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get weekly/monthly workload compliance for a single doctor.
    /// Working hours are calculated from completed consultation sessions only.
    /// </summary>
    [HttpGet("doctor-workload")]
    [ProducesResponseType(typeof(ApiResponse<DoctorWorkloadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDoctorWorkload(
        [FromQuery] Guid doctorId,
        [FromQuery] WorkloadPeriodType periodType = WorkloadPeriodType.Week,
        [FromQuery] DateOnly? date = null)
    {
        if (doctorId == Guid.Empty)
            return BadRequest(ApiResponseFactory.Error("doctorId is required."));

        var localToday = DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZoneResolver.TimeZone));

        var query = new GetDoctorWorkloadQuery
        {
            DoctorId = doctorId,
            PeriodType = periodType,
            Date = date ?? localToday
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get paged workload compliance list for doctors (dashboard view).
    /// </summary>
    [HttpGet("doctor-workloads")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DoctorWorkloadListItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDoctorWorkloads(
        [FromQuery] WorkloadPeriodType periodType = WorkloadPeriodType.Week,
        [FromQuery] DateOnly? date = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] OphthalmologistEmploymentType? employmentType = null,
        [FromQuery] string? status = null,
        [FromQuery] bool warningOnly = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var localToday = DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZoneResolver.TimeZone));

        var query = new GetDoctorWorkloadsQuery
        {
            PeriodType = periodType,
            Date = date ?? localToday,
            SearchTerm = searchTerm,
            EmploymentType = employmentType,
            Status = status,
            WarningOnly = warningOnly,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Manually executes the full-time rolling-window slot generation job once.
    /// This endpoint runs the same job logic as Hangfire recurring execution.
    /// </summary>
    [HttpPost("jobs/fulltime-slot-rolling-window/trigger-once")]
    [AuthorizePermission(Permissions.SchedulesManage)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TriggerFullTimeRollingWindowJobOnce(CancellationToken cancellationToken)
    {
        try
        {
            await _fullTimeSlotGenerationJob.ExecuteAsync(cancellationToken);

            return Ok(ApiResponseFactory.Success(new
            {
                JobId = "fulltime-slot-rolling-window",
                TriggeredAtUtc = DateTime.UtcNow
            }, "Triggered full-time rolling-window job successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to trigger full-time rolling-window job manually.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponseFactory.InternalServerError("Failed to trigger full-time rolling-window job."));
        }
    }

    /// <summary>
    /// Get today's clinic operations summary.
    /// </summary>
    [HttpGet("today-summary")]
    [ProducesResponseType(typeof(ApiResponse<TodaySummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTodaySummary(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTodaySummaryQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get today's slot utilization overview.
    /// </summary>
    [HttpGet("slot-utilization")]
    [ProducesResponseType(typeof(ApiResponse<SlotUtilizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSlotUtilization(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSlotUtilizationQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get the live patient queue for today.
    /// </summary>
    [HttpGet("live-queue")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<LiveQueueItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLiveQueue(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLiveQueueQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get current doctor status and workload for today.
    /// </summary>
    [HttpGet("doctor-status")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DoctorStatusDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDoctorStatus(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDoctorStatusQuery(), cancellationToken);
        return HandleResult(result);
    }
}
