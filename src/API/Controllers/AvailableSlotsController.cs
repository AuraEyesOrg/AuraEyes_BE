using Application.Common.Constants;
using Application.Common.Models;
using Application.Ophthalmologists.AvailableSlots.Commands.CreateAvailableSlot;
using Application.Ophthalmologists.AvailableSlots.Commands.DeleteAvailableSlot;
using Application.Ophthalmologists.AvailableSlots.Commands.UpdateAvailableSlot;
using Application.Ophthalmologists.AvailableSlots.Common;
using Application.Ophthalmologists.AvailableSlots.Queries.GetAvailableSlot;
using Application.Ophthalmologists.AvailableSlots.Queries.GetAvailableSlots;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Available slot management endpoints for ophthalmologists and organisations.
/// </summary>
[Route("api/available-slots")]
public class AvailableSlotsController : BaseApiController
{
    private readonly IMediator _mediator;

    public AvailableSlotsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get available slots with pagination and filtering.
    /// </summary>
    /// <param name="ophthalmologistId">Filter by ophthalmologist ID.</param>
    /// <param name="organisationId">Filter by organisation ID.</param>
    /// <param name="fromDate">Filter slots from this date.</param>
    /// <param name="toDate">Filter slots up to this date.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <returns>Paginated list of available slots.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AvailableSlotListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableSlots(
        [FromQuery] Guid? ophthalmologistId = null,
        [FromQuery] Guid? organisationId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAvailableSlotsQuery
        {
            OphthalmologistId = ophthalmologistId,
            OrganisationId = organisationId,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a specific available slot by ID.
    /// </summary>
    /// <param name="slotId">Available slot ID.</param>
    /// <returns>Available slot details.</returns>
    [HttpGet("{slotId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<AvailableSlotDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailableSlot(Guid slotId)
    {
        var result = await _mediator.Send(new GetAvailableSlotQuery(slotId));
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new available slot.
    /// </summary>
    /// <param name="request">Create available slot request.</param>
    /// <returns>Created available slot ID.</returns>
    [HttpPost]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAvailableSlot([FromBody] CreateAvailableSlotRequest request)
    {
        var command = new CreateAvailableSlotCommand
        {
            OrganisationId = request.OrganisationId,
            OphthalmologistId = request.OphthalmologistId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            MaxCapacity = request.MaxCapacity
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetAvailableSlot),
                new { slotId = result.Data },
                ApiResponseFactory.Success(result.Data, "Available slot created successfully."));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing available slot.
    /// </summary>
    /// <param name="slotId">Available slot ID.</param>
    /// <param name="request">Update available slot request.</param>
    /// <returns>Success status.</returns>
    [HttpPut("{slotId:guid}")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAvailableSlot(
        Guid slotId,
        [FromBody] UpdateAvailableSlotRequest request)
    {
        var command = new UpdateAvailableSlotCommand
        {
            AvailableSlotId = slotId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            MaxCapacity = request.MaxCapacity
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Available slot updated successfully.");
    }

    /// <summary>
    /// Delete an available slot.
    /// </summary>
    /// <param name="slotId">Available slot ID.</param>
    /// <returns>Success status.</returns>
    [HttpDelete("{slotId:guid}")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAvailableSlot(Guid slotId)
    {
        var result = await _mediator.Send(new DeleteAvailableSlotCommand { AvailableSlotId = slotId });
        return HandleResult(result, "Available slot deleted successfully.");
    }
}

/// <summary>
/// Request model for creating an available slot.
/// </summary>
public record CreateAvailableSlotRequest
{
    public Guid? OrganisationId { get; init; }
    public Guid? OphthalmologistId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int MaxCapacity { get; init; }
}

/// <summary>
/// Request model for updating an available slot.
/// </summary>
public record UpdateAvailableSlotRequest
{
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int MaxCapacity { get; init; }
}
