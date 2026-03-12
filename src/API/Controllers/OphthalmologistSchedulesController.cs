using Application.Common.Constants;
using Application.Common.Models;
using Application.Ophthalmologists.Schedules.Commands.CreateSchedule;
using Application.Ophthalmologists.Schedules.Commands.UpdateScheduleStatus;
using Application.Ophthalmologists.Schedules.Common;
using Application.Ophthalmologists.Schedules.Queries.GetSchedule;
using Application.Ophthalmologists.Schedules.Queries.GetSchedules;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Schedule management endpoints for ophthalmologists.
/// </summary>
[Route("api/ophthalmologists/{ophthalmologistId:guid}/schedules")]
public class OphthalmologistSchedulesController : BaseApiController
{
    private readonly IMediator _mediator;

    public OphthalmologistSchedulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get schedules for an ophthalmologist with pagination and filtering.
    /// </summary>
    /// <param name="ophthalmologistId">Ophthalmologist ID.</param>
    /// <param name="status">Filter by schedule status.</param>
    /// <param name="slotType">Filter by slot type.</param>
    /// <param name="fromDate">Filter schedules from this date.</param>
    /// <param name="toDate">Filter schedules up to this date.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <returns>Paginated list of schedules.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ScheduleListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSchedules(
        Guid ophthalmologistId,
        [FromQuery] ScheduleStatus? status = null,
        [FromQuery] SlotType? slotType = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetSchedulesQuery
        {
            OphthalmologistId = ophthalmologistId,
            Status = status,
            SlotType = slotType,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a specific schedule by ID.
    /// </summary>
    /// <param name="ophthalmologistId">Ophthalmologist ID.</param>
    /// <param name="scheduleId">Schedule ID.</param>
    /// <returns>Schedule details.</returns>
    [HttpGet("{scheduleId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ScheduleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSchedule(Guid ophthalmologistId, Guid scheduleId)
    {
        var result = await _mediator.Send(new GetScheduleQuery(scheduleId));
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new schedule (time slot) for an ophthalmologist.
    /// </summary>
    /// <param name="ophthalmologistId">Ophthalmologist ID.</param>
    /// <param name="request">Create schedule request.</param>
    /// <returns>Created schedule ID.</returns>
    [HttpPost]
    [Authorize(Policy = Policies.VerifiedOphthalmologist)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateSchedule(
        Guid ophthalmologistId,
        [FromBody] CreateScheduleRequest request)
    {
        var command = new CreateScheduleCommand
        {
            AvailableSlotId = request.AvailableSlotId,
            PatientId = request.PatientId,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotType = request.SlotType,
            Cost = request.Cost
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetSchedule),
                new { ophthalmologistId, scheduleId = result.Data },
                ApiResponseFactory.Success(result.Data, "Schedule created successfully."));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Update the status of a schedule.
    /// </summary>
    /// <param name="ophthalmologistId">Ophthalmologist ID.</param>
    /// <param name="scheduleId">Schedule ID.</param>
    /// <param name="request">Update status request.</param>
    /// <returns>Success status.</returns>
    [HttpPatch("{scheduleId:guid}/status")]
    [Authorize(Policy = Policies.VerifiedOphthalmologist)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateScheduleStatus(
        Guid ophthalmologistId,
        Guid scheduleId,
        [FromBody] UpdateScheduleStatusRequest request)
    {
        var command = new UpdateScheduleStatusCommand
        {
            ScheduleId = scheduleId,
            NewStatus = request.NewStatus
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Schedule status updated successfully.");
    }
}

/// <summary>
/// Request model for creating a schedule.
/// </summary>
public record CreateScheduleRequest
{
    public Guid AvailableSlotId { get; init; }
    public Guid PatientId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public SlotType SlotType { get; init; }
    public decimal? Cost { get; init; }
}

/// <summary>
/// Request model for updating schedule status.
/// </summary>
public record UpdateScheduleStatusRequest
{
    public ScheduleStatus NewStatus { get; init; }
}
