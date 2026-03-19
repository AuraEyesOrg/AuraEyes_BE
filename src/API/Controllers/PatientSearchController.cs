using Application.Common.Models;
using Application.Ophthalmologists.Common;
using Application.Ophthalmologists.Queries.GetOphthalmologist;
using Application.Ophthalmologists.Queries.GetOphthalmologists;
using Application.Scheduling.AppointmentSlots.Common;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlot;
using Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlots;
using Application.SystemAdmin.Organisations.Queries.GetOrganisations;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace API.Controllers;

/// <summary>
/// Patient-facing search endpoints for ophthalmologists, organisations and booking slots.
/// </summary>
[Route("api/patient/search")]
public class PatientSearchController : BaseApiController
{
    private readonly IMediator _mediator;

    public PatientSearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Search ophthalmologists with pagination.
    /// Only verified profiles are returned for patient-facing search.
    /// </summary>
    /// <param name="searchTerm">Search by name, email, bio, etc.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <returns>Paginated list of ophthalmologists.</returns>
    [HttpGet("ophthalmologists")]
    [AllowAnonymous]
    [OutputCache(PolicyName = "PublicData")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OphthalmologistListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchOphthalmologists(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetOphthalmologistsQuery
        {
            SearchTerm = searchTerm,
            IsVerified = true,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get ophthalmologist detail by ID for patient view.
    /// </summary>
    /// <param name="id">Ophthalmologist ID.</param>
    /// <returns>Ophthalmologist details.</returns>
    [HttpGet("ophthalmologists/{id:guid}")]
    [AllowAnonymous]
    [OutputCache(PolicyName = "PublicData")]
    [ProducesResponseType(typeof(ApiResponse<OphthalmologistDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOphthalmologistDetail(Guid id)
    {
        var result = await _mediator.Send(new GetOphthalmologistQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Search organisations (clinics/hospitals) with pagination.
    /// </summary>
    /// <param name="searchTerm">Search by name or address.</param>
    /// <param name="orgType">Filter by organisation type.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <returns>Paginated list of organisations.</returns>
    [HttpGet("organisations")]
    [AllowAnonymous]
    [OutputCache(PolicyName = "PublicData")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrganisationListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchOrganisations(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? orgType = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetOrganisationsQuery
        {
            SearchTerm = searchTerm,
            OrgType = orgType,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get available booking slots for a specific ophthalmologist or organisation.
    /// </summary>
    /// <param name="ophthalmologistId">Filter by ophthalmologist ID.</param>
    /// <param name="organisationId">Filter by organisation ID.</param>
    /// <param name="fromDate">Filter slots from this date.</param>
    /// <param name="toDate">Filter slots up to this date.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <returns>Paginated list of available slots.</returns>
    [HttpGet("available-slots")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AppointmentSlotListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAvailableSlots(
        [FromQuery] Guid? ophthalmologistId = null,
        [FromQuery] Guid? organisationId = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAppointmentSlotsQuery
        {
            OphthalId = ophthalmologistId,
            OrgId = organisationId,
            Status = ScheduleStatus.Available,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a specific available slot by ID for patient booking.
    /// </summary>
    /// <param name="slotId">Available slot ID.</param>
    /// <returns>Available slot details.</returns>
    [HttpGet("available-slots/{slotId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AppointmentSlotDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailableSlotDetail(Guid slotId)
    {
        var result = await _mediator.Send(new GetAppointmentSlotQuery(slotId));
        return HandleResult(result);
    }
}

