using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Organisations.Queries.GetOrganisations;

/// <summary>
/// Query to get organisations (clinics/hospitals) with pagination
/// Screen: 3.4.1 View Clinic & Device Inventory Overview
/// </summary>
public record GetOrganisationsQuery : IQuery<PagedResult<OrganisationListDto>>
{
    public string? SearchTerm { get; init; }
    public string? OrgType { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
