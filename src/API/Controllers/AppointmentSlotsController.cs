using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Commands.BlockSlot;
using Application.Scheduling.AppointmentSlots.Commands.CreateAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Commands.DeleteAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Commands.GenerateSlots;
using Application.Scheduling.AppointmentSlots.Commands.UnblockSlot;
using Application.Scheduling.AppointmentSlots.Common;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlots;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlotStats;
using Domain.Enums;
using Infrastructure.Identity.Authorization;
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
    [AuthorizePermission(Permissions.AppointmentsRead)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AppointmentSlotListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppointmentSlots(
        [FromQuery] Guid? scheduleTemplateId = null,
        [FromQuery] Guid? ophthalId = null,
        [FromQuery] ScheduleStatus? status = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] bool excludePastSlots = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAppointmentSlotsQuery
        {
            ScheduleTemplateId = scheduleTemplateId,
            OphthalId = ophthalId,
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            ExcludePastSlots = excludePastSlots,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get available appointment slots for a given date (patient-facing).
    /// </summary>
    [HttpGet("available")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<AvailableSlotDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableSlots(
        [FromQuery] DateOnly? date = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 100)
    {
        var query = new GetAppointmentSlotsQuery
        {
            Status = ScheduleStatus.Available,
            ExcludePastSlots = true,
            FromDate = date,
            ToDate = date,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess || result.Data == null)
            return HandleResult(result);

        var slots = result.Data.Items
            .Select(x => new AvailableSlotDto
            {
                SlotId = x.Id,
                Date = x.Date.ToString("yyyy-MM-dd"),
                StartTime = x.StartTime.ToString("HH:mm"),
                EndTime = x.EndTime.ToString("HH:mm"),
                Remaining = x.AvailableCapacity,
                MaxCapacity = x.MaxCapacity,
                Cost = x.Cost
            })
            .ToList();

        return Ok(ApiResponseFactory.Success(slots));
    }

    /// <summary>
    /// Get a specific appointment slot by ID.
    /// </summary>
    [HttpGet("{slotId:guid}")]
    [AuthorizePermission(Permissions.AppointmentsRead)]
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
    [AuthorizePermission(Permissions.ApptSlotsManage)]
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
            EndTime = request.EndTime
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
    [AuthorizePermission(Permissions.AppointmentsRead)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentSlotStatsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentSlotStats()
    {
        var query = new GetAppointmentSlotStatsQuery();

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }


    /// <summary>
    /// Delete (cancel) an appointment slot.
    /// </summary>
    [HttpDelete("{slotId:guid}")]
    [AuthorizePermission(Permissions.ApptSlotsManage)]
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
    /// Generate appointment slots from a schedule template for a date range.
    /// </summary>
    [HttpPost("generate")]
    [AuthorizePermission(Permissions.ApptSlotsManage)]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateSlots([FromBody] GenerateSlotsRequest request)
    {
        var command = new GenerateSlotsCommand
        {
            ScheduleTemplateId = request.ScheduleTemplateId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            SkipExistingDates = request.SkipExistingDates
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }



    /// <summary>
    /// Block an appointment slot (doctor not available).
    /// </summary>
    [HttpPost("{slotId:guid}/block")]
    [AuthorizePermission(Permissions.ApptSlotsManage)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> BlockSlot(Guid slotId, [FromBody] BlockSlotRequest request)
    {
        var command = new BlockSlotCommand
        {
            AppointmentSlotId = slotId,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Unblock an appointment slot (make available again).
    /// </summary>
    [HttpPost("{slotId:guid}/unblock")]
    [AuthorizePermission(Permissions.ApptSlotsManage)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnblockSlot(Guid slotId)
    {
        var command = new UnblockSlotCommand
        {
            AppointmentSlotId = slotId
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
}

public record GenerateSlotsRequest
{
    public Guid ScheduleTemplateId { get; init; }
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
    public bool SkipExistingDates { get; init; } = true;
}



public record BlockSlotRequest
{
    public string? Reason { get; init; }
}

public record UnblockSlotRequest;
