using Application.Common.Constants;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Commands.BookAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Commands.CreateAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Commands.DeleteAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlotCost;
using Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlotStatus;
using Application.Scheduling.AppointmentSlots.Common;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlots;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlotStats;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Appointment slot management endpoints for booking and scheduling.
/// </summary>
[Route("api/appointment-slots")]
public class AppointmentSlotsController : BaseApiController
{
    private readonly IMediator _mediator;

    public AppointmentSlotsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get appointment slots with pagination and filtering.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AppointmentSlotListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppointmentSlots(
        [FromQuery] Guid? scheduleTemplateId = null,
        [FromQuery] Guid? ophthalId = null,
        [FromQuery] Guid? orgId = null,
        [FromQuery] ScheduleStatus? status = null,
        [FromQuery] SlotType? slotType = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAppointmentSlotsQuery
        {
            ScheduleTemplateId = scheduleTemplateId,
            OphthalId = ophthalId,
            OrgId = orgId,
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
    /// Get a specific appointment slot by ID.
    /// </summary>
    [HttpGet("{slotId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<AppointmentSlotDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentSlot(Guid slotId)
    {
        var result = await _mediator.Send(new GetAppointmentSlotQuery(slotId));
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new appointment slot.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Policies.OphthalmologistOrOrgAdmin)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateAppointmentSlot([FromBody] CreateAppointmentSlotRequest request)
    {
        var command = new CreateAppointmentSlotCommand
        {
            ScheduleTemplateId = request.ScheduleTemplateId,
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
                nameof(GetAppointmentSlot),
                new { slotId = result.Data },
                ApiResponseFactory.Success(result.Data, "Appointment slot created successfully."));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get appointment slot statistics by ophthalmologist or organisation.
    /// </summary>
    [HttpGet("stats")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<AppointmentSlotStatsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentSlotStats(
        [FromQuery] Guid? ophthalId = null,
        [FromQuery] Guid? orgId = null)
    {
        var query = new GetAppointmentSlotStatsQuery
        {
            OphthalId = ophthalId,
            OrgId = orgId
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing appointment slot.
    /// </summary>
    [HttpPut("{slotId:guid}")]
    [Authorize(Policy = Policies.OphthalmologistOrOrgAdmin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAppointmentSlot(Guid slotId, [FromBody] UpdateAppointmentSlotRequest request)
    {
        var command = new UpdateAppointmentSlotCommand
        {
            AppointmentSlotId = slotId,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotType = request.SlotType,
            Cost = request.Cost
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete (cancel) an appointment slot.
    /// </summary>
    [HttpDelete("{slotId:guid}")]
    [Authorize(Policy = Policies.OphthalmologistOrOrgAdmin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAppointmentSlot(Guid slotId)
    {
        var command = new DeleteAppointmentSlotCommand { AppointmentSlotId = slotId };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Book an appointment slot.
    /// </summary>
    [HttpPost("{slotId:guid}/book")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> BookAppointmentSlot(Guid slotId, [FromBody] BookAppointmentSlotRequest request)
    {
        var command = new BookAppointmentSlotCommand
        {
            AppointmentSlotId = slotId,
            PatientId = request.PatientId
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update appointment slot status.
    /// </summary>
    [HttpPatch("{slotId:guid}/status")]
    [Authorize(Policy = Policies.OphthalmologistOrOrgAdmin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAppointmentSlotStatus(Guid slotId, [FromBody] UpdateAppointmentSlotStatusRequest request)
    {
        var command = new UpdateAppointmentSlotStatusCommand
        {
            AppointmentSlotId = slotId,
            NewStatus = request.NewStatus
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update appointment slot cost.
    /// </summary>
    [HttpPatch("{slotId:guid}/cost")]
    [Authorize(Policy = Policies.OphthalmologistOrOrgAdmin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAppointmentSlotCost(Guid slotId, [FromBody] UpdateAppointmentSlotCostRequest request)
    {
        var command = new UpdateAppointmentSlotCostCommand
        {
            AppointmentSlotId = slotId,
            Cost = request.Cost
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}

public record CreateAppointmentSlotRequest
{
    public Guid ScheduleTemplateId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public SlotType SlotType { get; init; }
    public decimal? Cost { get; init; }
}

public record UpdateAppointmentSlotRequest
{
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public SlotType SlotType { get; init; }
    public decimal? Cost { get; init; }
}

public record BookAppointmentSlotRequest
{
    public Guid PatientId { get; init; }
}

public record UpdateAppointmentSlotStatusRequest
{
    public ScheduleStatus NewStatus { get; init; }
}

public record UpdateAppointmentSlotCostRequest
{
    public decimal? Cost { get; init; }
}
