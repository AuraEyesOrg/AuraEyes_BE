using Application.Common.Interfaces;
using Application.SystemAdmin.Organisations.Queries.GetOrganisations;

namespace Application.SystemAdmin.Organisations.Queries.GetOrganisationById;

public record GetOrganisationByIdQuery(Guid Id) : IQuery<OrganisationListDto>;
