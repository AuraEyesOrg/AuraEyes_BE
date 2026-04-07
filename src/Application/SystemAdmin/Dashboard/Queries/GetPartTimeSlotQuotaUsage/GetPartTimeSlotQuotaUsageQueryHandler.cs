using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemSettings.Interfaces;
using Domain.Repositories;

namespace Application.SystemAdmin.Dashboard.Queries.GetPartTimeSlotQuotaUsage;

public class GetPartTimeSlotQuotaUsageQueryHandler
    : IQueryHandler<GetPartTimeSlotQuotaUsageQuery, IReadOnlyList<PartTimeSlotQuotaUsageDto>>
{
    private readonly IDailySlotQuotaRepository _dailySlotQuotaRepository;
    private readonly ISystemSettingService _settingService;

    public GetPartTimeSlotQuotaUsageQueryHandler(
        IDailySlotQuotaRepository dailySlotQuotaRepository,
        ISystemSettingService settingService)
    {
        _dailySlotQuotaRepository = dailySlotQuotaRepository;
        _settingService = settingService;
    }

    public async Task<Result<IReadOnlyList<PartTimeSlotQuotaUsageDto>>> Handle(
        GetPartTimeSlotQuotaUsageQuery request,
        CancellationToken cancellationToken)
    {
        if (request.ToDate < request.FromDate)
        {
            return Result<IReadOnlyList<PartTimeSlotQuotaUsageDto>>.Failure(
                "ToDate must be greater than or equal to FromDate.");
        }

        var configuredQuota = await GetPartTimeDailyQuotaAsync(cancellationToken);
        var rows = await _dailySlotQuotaRepository.GetByDateRangeAsync(
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var byDate = rows.ToDictionary(r => r.Date);
        var result = new List<PartTimeSlotQuotaUsageDto>();

        var current = request.FromDate;
        while (current <= request.ToDate)
        {
            if (byDate.TryGetValue(current, out var row))
            {
                result.Add(new PartTimeSlotQuotaUsageDto
                {
                    Date = current,
                    UsedSlots = row.PartTimeSlotCount,
                    Quota = row.QuotaSnapshot,
                    RemainingSlots = row.Remaining
                });
            }
            else
            {
                result.Add(new PartTimeSlotQuotaUsageDto
                {
                    Date = current,
                    UsedSlots = 0,
                    Quota = configuredQuota,
                    RemainingSlots = configuredQuota
                });
            }

            current = current.AddDays(1);
        }

        return Result<IReadOnlyList<PartTimeSlotQuotaUsageDto>>.Success(result);
    }

    private async Task<int> GetPartTimeDailyQuotaAsync(CancellationToken cancellationToken)
    {
        var configured = await _settingService.GetSettingAsync(SystemSettingKeys.PartTimeMaxSlotsPerDay, cancellationToken);
        return int.TryParse(configured, out var value) && value > 0 ? value : 100;
    }
}