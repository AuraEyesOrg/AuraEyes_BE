using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Organisations.Common;
using Domain.Enums;

namespace Application.Organisations.Queries.GetOrganisations;

/// <summary>
/// Query to get organisations with pagination and filtering.
/// </summary>
public record GetOrganisationsQuery : IQuery<PagedResult<OrganisationListDto>>
{
    /// <summary>
    /// Search term for filtering by name, address, or license number.
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by organisation type.
    /// </summary>
    public OrgType? OrgType { get; init; }

    /// <summary>
    /// Filter by active status.
    /// </summary>
    public bool? IsActive { get; init; }

    /// <summary>
    /// Page number (default: 1).
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Page size (default: 10).
    /// </summary>
    public int PageSize { get; init; } = 10;
}
