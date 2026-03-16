using Application.Common.Constants;
using Application.Common.Models;
using Application.Common.Interfaces;
using Application.Organisations.Queries.GetDashboardMetrics;
using Application.Scheduling.Appointments.Common;
using Application.Scheduling.Appointments.Queries.GetOrganisationAppointments;
using Application.Scheduling.Appointments.Queries.GetOrganisationAvailableSlots;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/organisations")]
public class OrganisationsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public OrganisationsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("dashboard-metrics")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<OrganisationDashboardMetricsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(new GetDashboardMetricsQuery(_currentUserService.UserId.Value));
        return HandleResult(result);
    }

    [HttpGet("{orgId:guid}/available-slots")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrganisationAvailableSlotDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganisationAvailableSlots(
        Guid orgId,
        [FromQuery] DateOnly? date = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        var query = new GetOrganisationAvailableSlotsQuery
        {
            OrganisationId = orgId,
            Date = date,
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{orgId:guid}/appointments")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ClinicAppointmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganisationAppointments(
        Guid orgId,
        [FromQuery] DateOnly? date = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] AppointmentStatus? status = null)
    {
        var query = new GetOrganisationAppointmentsQuery
        {
            OrganisationId = orgId,
            Date = date,
            FromDate = fromDate,
            ToDate = toDate,
            Status = status
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }
}
