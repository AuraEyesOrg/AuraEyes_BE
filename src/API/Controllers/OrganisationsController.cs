using Application.Common.Constants;
using Application.Common.Models;
using Application.Organisations.Commands.CreateOrganisation;
using Application.Organisations.Commands.DeleteOrganisation;
using Application.Organisations.Commands.UpdateOrganisation;
using Application.Organisations.Common;
using Application.Organisations.Queries.GetOrganisation;
using Application.Organisations.Queries.GetOrganisations;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Organisation management endpoints.
/// Provides CRUD operations for organisation profiles.
/// </summary>
public class OrganisationsController : BaseApiController
{
    private readonly IMediator _mediator;

    public OrganisationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get organisations with pagination and filtering.
    /// </summary>
    /// <param name="searchTerm">Search term for filtering by name, address, or license number.</param>
    /// <param name="orgType">Filter by organisation type (Hospital, Clinic, PrivatePractice, ResearchCenter, DiagnosticCenter).</param>
    /// <param name="isActive">Filter by active status.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <returns>Paginated list of organisations.</returns>
    [HttpGet]
    [Authorize]
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
    /// <returns>Organisation details.</returns>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<OrganisationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganisation(Guid id)
    {
        var result = await _mediator.Send(new GetOrganisationQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new organisation.
    /// </summary>
    /// <param name="command">Create organisation command.</param>
    /// <returns>Created organisation ID.</returns>
    [HttpPost]
    [Authorize(Policy = Policies.AdminsOnly)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateOrganisation([FromBody] CreateOrganisationCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetOrganisation),
                new { id = result.Data },
                ApiResponseFactory.Success(result.Data, "Organisation created successfully."));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing organisation.
    /// </summary>
    /// <param name="id">Organisation ID.</param>
    /// <param name="command">Update organisation command.</param>
    /// <returns>Success response.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.AdminsOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateOrganisation(Guid id, [FromBody] UpdateOrganisationCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponseFactory.Error("ID in URL does not match ID in request body."));
        }

        var result = await _mediator.Send(command);
        return HandleResult(result, "Organisation updated successfully.");
    }

    /// <summary>
    /// Delete (soft-delete) an organisation.
    /// </summary>
    /// <param name="id">Organisation ID.</param>
    /// <returns>Success response.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.SystemAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrganisation(Guid id)
    {
        var result = await _mediator.Send(new DeleteOrganisationCommand(id));
        return HandleResult(result, "Organisation deleted successfully.");
    }
}
