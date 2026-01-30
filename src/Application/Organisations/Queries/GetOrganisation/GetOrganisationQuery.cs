using Application.Common.Interfaces;
using Application.Organisations.Common;

namespace Application.Organisations.Queries.GetOrganisation;

/// <summary>
/// Query to get a single organisation by ID.
/// </summary>
public record GetOrganisationQuery(Guid Id) : IQuery<OrganisationDto>;
