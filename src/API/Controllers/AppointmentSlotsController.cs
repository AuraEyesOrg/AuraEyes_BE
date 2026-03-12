using Application.Common.Constants;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Commands.CreateAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Common;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlots;
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
