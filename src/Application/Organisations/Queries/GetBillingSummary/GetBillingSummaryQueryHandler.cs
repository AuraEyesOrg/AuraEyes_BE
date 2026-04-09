using Application.Common.Interfaces;
using Application.Common.Models;
using Application.OrganisationScreenings;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Organisations.Queries.GetBillingSummary;

public sealed class GetBillingSummaryQueryHandler
    : IQueryHandler<GetBillingSummaryQuery, OrgBillingSummaryDto>
{
    private readonly IRepository<Organisation> _orgRepo;
    private readonly IOrganisationPatientsRepository _orgPatientsRepo;

    public GetBillingSummaryQueryHandler(
        IRepository<Organisation> orgRepo,
        IOrganisationPatientsRepository orgPatientsRepo)
    {
        _orgRepo = orgRepo;
        _orgPatientsRepo = orgPatientsRepo;
    }

    public async Task<Result<OrgBillingSummaryDto>> Handle(
        GetBillingSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var orgs = await _orgRepo.FindAsync(o => o.OwnerId == request.OrgAdminUserId, cancellationToken);
        var org = orgs.FirstOrDefault();

        if (org is null)
            return Result<OrgBillingSummaryDto>.NotFound("Organisation not found.");

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var screeningCounts = await _orgPatientsRepo.GetScreeningCountsForOrganisationAsync(
            org.Id, monthStart, cancellationToken);

        return Result<OrgBillingSummaryDto>.Success(new OrgBillingSummaryDto
        {
            TotalScreeningsThisMonth = screeningCounts.TotalScreeningsFromDate,
            TotalScreeningsAllTime = screeningCounts.TotalScreeningsAllTime,
            RemainingQuota = org.PurchasedAiQuota,
            UsedQuotaToday = org.UsedAiQuota,
            PurchasedQuota = org.PurchasedAiQuota
        });
    }
}
