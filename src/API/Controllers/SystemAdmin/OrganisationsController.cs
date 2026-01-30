using Application.Common.Constants;
using Application.Common.Models;
using Application.Organisations.Common;
using Application.Organisations.Queries.GetOrganisation;
using Application.Organisations.Queries.GetOrganisations;
using Application.SystemAdmin.Organisations.Queries.GetOrganisationMetrics;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// System Admin Organisation Management endpoints.
/// Provides centralized view and management of registered organisations (clinics/hospitals).
/// </summary>
[Route("api/system-admin/[controller]")]
[Authorize(Policy = Policies.SystemAdminOnly)]
public class OrganisationsController : BaseApiController
{
    private readonly IMediator _mediator;

    public OrganisationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get organisation metrics.
    /// </summary>
    /// <remarks>
    /// Screen: 3.4.1-3.4.3 View Clinic and Device Inventory Overview
    /// </remarks>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(ApiResponse<OrganisationMetricsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMetrics()
    {
        var result = await _mediator.Send(new GetOrganisationMetricsQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Get organisations with pagination and filtering.
    /// </summary>
    /// <param name="searchTerm">Search by name, address, or license number.</param>
    /// <param name="orgType">Filter by organisation type (Hospital, Clinic, PrivatePractice, ResearchCenter, DiagnosticCenter).</param>
    /// <param name="isActive">Filter by active status.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <remarks>
    /// Screen: 3.4.6-3.4.8 Search Clinics/Hospitals, Filter by Type
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrganisationListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrganisations(
        [FromQuery] string? searchTerm = null,
        [FromQuery] OrgType? orgType = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetOrganisationsQuery
        {
            SearchTerm = searchTerm,
            OrgType = orgType,
            IsActive = isActive,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get organisation by ID.
    /// </summary>
    /// <param name="id">Organisation ID.</param>
    /// <remarks>
    /// Screen: 3.4.9 View Organisation Details
    /// </remarks>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrganisationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganisation(Guid id)
    {
        var result = await _mediator.Send(new GetOrganisationQuery(id));
        return HandleResult(result);
    }
}
