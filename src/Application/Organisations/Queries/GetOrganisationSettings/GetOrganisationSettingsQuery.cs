using Application.Common.Interfaces;
using Application.Organisations.Common;

namespace Application.Organisations.Queries.GetOrganisationSettings;

public record GetOrganisationSettingsQuery(Guid OrgAdminUserId) : IQuery<OrganisationSettingsDto>;
