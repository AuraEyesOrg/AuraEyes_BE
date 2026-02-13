using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.Organisations.Queries.GetOrganisationMetrics;
using Application.SystemAdmin.Organisations.Queries.GetOrganisations;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// System Admin Organisation Management endpoints
/// Provides centralized view and management of registered organisations (clinics/hospitals).
/// Uses existing Organisation entity from Domain.
/// </summary>
[Route("api/system-admin/[controller]")]
[Authorize(Policy = Policies.SystemAdminOnly)]
public class OrganisationsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IRepository<Organisation> _organisationRepository;

    public OrganisationsController(IMediator mediator, IRepository<Organisation> organisationRepository)
    {
        _mediator = mediator;
        _organisationRepository = organisationRepository;
    }

    /// <summary>
    /// Get organisation metrics
    /// </summary>
    /// <remarks>
    /// Screen: 3.4.1-3.4.3 View Clinic & Device Inventory Overview
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
    /// Get organisations with pagination and filtering
    /// </summary>
    /// <param name="searchTerm">Search by name or address</param>
    /// <param name="orgType">Filter by organisation type (Hospital, Clinic, PrivatePractice, etc.)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <remarks>
    /// Screen: 3.4.6-3.4.8 Search Clinics/Hospitals, Filter by Type
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrganisationListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrganisations(
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
    /// Get organisation by ID
    /// </summary>
    /// <param name="id">Organisation ID</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrganisationListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganisation(Guid id)
    {
        var org = await _organisationRepository.GetByIdAsync(id);
        if (org == null)
        {
            return NotFound(ApiResponseFactory.NotFound("Organisation not found"));
        }

        var dto = new OrganisationListDto
        {
            Id = org.Id,
            Name = org.Name,
            Address = org.Address,
            LicenseNumber = org.LicenseNumber,
            OrgType = org.OrgType.ToString(),
            DeviceCount = 0,
            IsActive = !org.IsDeleted,
            CreatedAt = org.CreatedAt
        };

        return Ok(ApiResponseFactory.Success(dto));
    }
}
