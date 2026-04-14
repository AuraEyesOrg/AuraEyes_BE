using Application.Common.Interfaces;

namespace Application.SystemAdmin.Organisations.Commands.UpdateMonthlyQuota;

public record UpdateOrganisationMonthlyQuotaCommand : ICommand
{
    public Guid OrganisationId { get; init; }
    public int MonthlyQuotaLimit { get; init; }
}
