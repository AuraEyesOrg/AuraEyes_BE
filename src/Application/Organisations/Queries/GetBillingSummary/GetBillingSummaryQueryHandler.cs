using Application.AiQuota.Interfaces;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.OrganisationScreenings;
using Application.SystemSettings.Interfaces;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;
using System.Globalization;

namespace Application.Organisations.Queries.GetBillingSummary;

public sealed class GetBillingSummaryQueryHandler
    : IQueryHandler<GetBillingSummaryQuery, OrgBillingSummaryDto>
{
    private readonly IRepository<Organisation> _orgRepo;
    private readonly IOrganisationPatientsRepository _orgPatientsRepo;
    private readonly IWalletRepository _walletRepository;
    private readonly IAiQuotaService _aiQuotaService;
    private readonly ISystemSettingService _settingService;

    public GetBillingSummaryQueryHandler(
        IRepository<Organisation> orgRepo,
        IOrganisationPatientsRepository orgPatientsRepo,
        IWalletRepository walletRepository,
        IAiQuotaService aiQuotaService,
        ISystemSettingService settingService)
    {
        _orgRepo = orgRepo;
        _orgPatientsRepo = orgPatientsRepo;
        _walletRepository = walletRepository;
        _aiQuotaService = aiQuotaService;
        _settingService = settingService;
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

        var quota = await _aiQuotaService.GetQuotaAsync(
            request.OrgAdminUserId,
            Roles.OrgAdmin,
            cancellationToken);

        var configuredUnitPrice = await _settingService.GetSettingAsync("AI_QUOTA_UNIT_PRICE", cancellationToken);
        var patientUnitPrice = decimal.TryParse(
                configuredUnitPrice,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var parsedPrice)
            && parsedPrice > 0m
                ? parsedPrice
                : 10000m;

        var organisationUnitPrice = quota.UnitPrice
            ?? Math.Round(patientUnitPrice * 0.60m, 0, MidpointRounding.AwayFromZero);
        var wallet = await _walletRepository.GetByUserIdAsync(request.OrgAdminUserId, cancellationToken);

        return Result<OrgBillingSummaryDto>.Success(new OrgBillingSummaryDto
        {
            TotalScreeningsThisMonth = screeningCounts.TotalScreeningsFromDate,
            TotalScreeningsAllTime = screeningCounts.TotalScreeningsAllTime,
            WalletBalance = wallet?.Balance ?? 0m,
            RemainingQuota = quota.RemainingQuota,
            MonthlyQuotaLimit = quota.MonthlyQuotaLimit ?? 0,
            MonthlyQuotaUsed = quota.MonthlyQuotaUsed ?? 0,
            MonthlyQuotaRemaining = quota.MonthlyQuotaRemaining ?? 0,
            UsedQuotaToday = quota.MonthlyQuotaUsed ?? 0,
            PurchasedQuota = org.PurchasedAiQuota,
            PatientUnitPrice = patientUnitPrice,
            OrganisationUnitPrice = organisationUnitPrice
        });
    }
}
