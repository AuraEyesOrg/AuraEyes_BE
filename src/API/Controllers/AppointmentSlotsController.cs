using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Commands.BlockSlot;
using Application.Scheduling.AppointmentSlots.Commands.BookAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Commands.ConfirmReservation;
using Application.Scheduling.AppointmentSlots.Commands.CreateAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Commands.DeleteAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Commands.GenerateSlots;
using Application.Scheduling.AppointmentSlots.Commands.ReleaseReservation;
using Application.Scheduling.AppointmentSlots.Commands.ReserveSlot;
using Application.Scheduling.AppointmentSlots.Commands.UnblockSlot;
using Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlotCost;
using Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlotStatus;
using Application.Scheduling.AppointmentSlots.Common;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlots;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlotStats;
using Application.Scheduling.AppointmentSlots.Queries.GetAllowedPriceRange;
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
    private readonly ICurrentUserService _currentUser;

    public AppointmentSlotsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
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
            OrgId = orgId,
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
    /// Get allowed cost range for an ophthalmologist based on years of experience.
    /// </summary>
    [HttpGet("ophthalmologists/{ophthalmologistId:guid}/pricing-range")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<AllowedPriceRangeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllowedPriceRange(Guid ophthalmologistId)
    {
        var result = await _mediator.Send(new GetAllowedPriceRangeQuery(ophthalmologistId));
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
    /// Book an appointment slot (slotId provided in request body).
    /// This endpoint is convenient for external assistants (e.g., n8n) that prefer fixed URLs.
    /// PatientId is resolved from the authenticated user's profile_id claim.
    /// </summary>
    [HttpPost("book")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BookAppointmentSlotByBody([FromBody] BookAppointmentSlotByBodyRequest request)
    {
        if (request is null)
        {
            return BadRequest(ApiResponseFactory.Error("Request body is required."));
        }

        if (!_currentUser.ProfileId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized(
                "Authenticated patient profile is required to book an appointment slot."));
        }

        var command = new BookAppointmentSlotCommand
        {
            AppointmentSlotId = request.SlotId,
            PatientId = _currentUser.ProfileId.Value
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Reserve a slot for the authenticated patient (slotId provided in request body).
    /// Uses row-locking and transaction handling from the ReserveSlotCommand handler.
    /// </summary>
    [HttpPost("reserve")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<ReserveSlotResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReserveSlotByBody([FromBody] ReserveSlotByBodyRequest request)
    {
        if (request is null)
        {
            return BadRequest(ApiResponseFactory.Error("Request body is required."));
        }

        if (!_currentUser.ProfileId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized(
                "Authenticated patient profile is required to reserve an appointment slot."));
        }

        var command = new ReserveSlotCommand
        {
            AppointmentSlotId = request.SlotId,
            PatientId = _currentUser.ProfileId.Value,
            ReservationMinutes = request.ReservationMinutes
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Confirm a reserved slot after payment (slotId provided in request body).
    /// Deducts wallet balance (if needed) and creates a ConsultationSession in a single transaction.
    /// Requires explicit consent flags for sharing AI results and retinal images with the doctor.
    /// </summary>
    [HttpPost("confirm")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<ConfirmReservationResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConfirmReservationByBody([FromBody] ConfirmReservationByBodyRequest request)
    {
        if (request is null)
        {
            return BadRequest(ApiResponseFactory.Error("Request body is required."));
        }

        if (!_currentUser.ProfileId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized(
                "Authenticated patient profile is required to confirm a reservation."));
        }

        // Enforce explicit consent: client/n8n must send both flags (no implicit defaults).
        if (request.ShareAiResults is null || request.ShareRetinalImages is null)
        {
            return BadRequest(ApiResponseFactory.Error(
                "Consent is required. Please specify both 'shareAiResults' and 'shareRetinalImages'."));
        }

        var command = new ConfirmReservationCommand
        {
            AppointmentSlotId = request.SlotId,
            PatientId = _currentUser.ProfileId.Value,
            AiScreeningId = request.AiScreeningId,
            ShareAiResults = request.ShareAiResults.Value,
            ShareRetinalImages = request.ShareRetinalImages.Value
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Release a reserved slot (slotId provided in request body).
    /// </summary>
    [HttpPost("release")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReleaseReservationByBody([FromBody] ReleaseReservationByBodyRequest request)
    {
        if (request is null)
        {
            return BadRequest(ApiResponseFactory.Error("Request body is required."));
        }

        if (!_currentUser.ProfileId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized(
                "Authenticated patient profile is required to release a reservation."));
        }

        var command = new ReleaseReservationCommand
        {
            AppointmentSlotId = request.SlotId,
            PatientId = _currentUser.ProfileId.Value,
            IsSystemRelease = false
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

    /// <summary>
    /// Generate appointment slots from a schedule template for a date range.
    /// </summary>
    [HttpPost("generate")]
    [Authorize(Policy = Policies.OphthalmologistOrOrgAdmin)]
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
    /// Reserve an appointment slot for a patient (starts reservation timer).
    /// </summary>
    [HttpPost("{slotId:guid}/reserve")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<ReserveSlotResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReserveSlot(Guid slotId, [FromBody] ReserveSlotRequest request)
    {
        var command = new ReserveSlotCommand
        {
            AppointmentSlotId = slotId,
            PatientId = request.PatientId,
            ReservationMinutes = request.ReservationMinutes
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Confirm a slot reservation after payment (creates consultation session).
    /// </summary>
    [HttpPost("{slotId:guid}/confirm")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<ConfirmReservationResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmReservation(Guid slotId, [FromBody] ConfirmReservationRequest request)
    {
        var command = new ConfirmReservationCommand
        {
            AppointmentSlotId = slotId,
            PatientId = request.PatientId,
            AiScreeningId = request.AiScreeningId,
            ShareRetinalImages = request.ShareRetinalImages,
            ShareAiResults = request.ShareAiResults
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Release a slot reservation (cancel before payment).
    /// </summary>
    [HttpPost("{slotId:guid}/release")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReleaseReservation(Guid slotId, [FromBody] ReleaseReservationRequest request)
    {
        var command = new ReleaseReservationCommand
        {
            AppointmentSlotId = slotId,
            PatientId = request.PatientId,
            IsSystemRelease = false
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Block an appointment slot (doctor not available).
    /// </summary>
    [HttpPost("{slotId:guid}/block")]
    [Authorize(Policy = Policies.OphthalmologistOrOrgAdmin)]
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
            OphthalmologistId = request.OphthalmologistId,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Unblock an appointment slot (make available again).
    /// </summary>
    [HttpPost("{slotId:guid}/unblock")]
    [Authorize(Policy = Policies.OphthalmologistOrOrgAdmin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnblockSlot(Guid slotId, [FromBody] UnblockSlotRequest request)
    {
        var command = new UnblockSlotCommand
        {
            AppointmentSlotId = slotId,
            OphthalmologistId = request.OphthalmologistId
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
    public decimal? Cost { get; init; }
}

public record UpdateAppointmentSlotRequest
{
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public decimal? Cost { get; init; }
}

public record BookAppointmentSlotRequest
{
    public Guid PatientId { get; init; }
}

public record BookAppointmentSlotByBodyRequest
{
    public Guid SlotId { get; init; }
}

public record ReserveSlotByBodyRequest
{
    public Guid SlotId { get; init; }
    public int ReservationMinutes { get; init; } = 5;
}

public record ConfirmReservationByBodyRequest
{
    public Guid SlotId { get; init; }
    public Guid? AiScreeningId { get; init; }
    public bool? ShareRetinalImages { get; init; }
    public bool? ShareAiResults { get; init; }
}

public record ReleaseReservationByBodyRequest
{
    public Guid SlotId { get; init; }
}

public record UpdateAppointmentSlotStatusRequest
{
    public ScheduleStatus NewStatus { get; init; }
}

public record UpdateAppointmentSlotCostRequest
{
    public decimal? Cost { get; init; }
}

public record GenerateSlotsRequest
{
    public Guid ScheduleTemplateId { get; init; }
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
    public bool SkipExistingDates { get; init; } = true;
}

public record ReserveSlotRequest
{
    public Guid PatientId { get; init; }
    public int ReservationMinutes { get; init; } = 5;
}

public record ConfirmReservationRequest
{
    public Guid PatientId { get; init; }
    public Guid? AiScreeningId { get; init; }
    public bool ShareRetinalImages { get; init; }
    public bool ShareAiResults { get; init; }
}

public record ReleaseReservationRequest
{
    public Guid PatientId { get; init; }
}

public record BlockSlotRequest
{
    public Guid OphthalmologistId { get; init; }
    public string? Reason { get; init; }
}

public record UnblockSlotRequest
{
    public Guid OphthalmologistId { get; init; }
}
