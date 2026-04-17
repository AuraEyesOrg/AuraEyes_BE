using System.Globalization;
using Application.Scheduling.ScheduleTemplates.Interfaces;
using Application.SystemSettings.Interfaces;
using Domain.Entities.Users;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Creates missing weekday system-generated schedule templates for full-time ophthalmologists.
/// Uses INSERT ... ON CONFLICT DO NOTHING for idempotent, concurrency-safe provisioning.
/// </summary>
public class FullTimeTemplateProvisioningService : IFullTimeTemplateProvisioningService
{
    private const string MinSlotCostSettingKey = "FULLTIME_MIN_SLOT_COST";
    private const string MaxSlotCostSettingKey = "FULLTIME_MAX_SLOT_COST";

    private const decimal DefaultMinSlotCost = 100000m;
    private const decimal DefaultMaxSlotCost = 400000m;

    private const int DefaultSlotDurationMinutes = 30;
    private const int DefaultMaxCapacity = 1;

    private static readonly TimeOnly DefaultStartTime = new(8, 0);
    private static readonly TimeOnly DefaultEndTime = new(17, 0);

    private static readonly DayOfWeek[] DefaultWeekdays =
    {
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday
    };

    private readonly ApplicationDbContext _context;
    private readonly ISystemSettingService _settingService;
    private readonly ILogger<FullTimeTemplateProvisioningService> _logger;

    public FullTimeTemplateProvisioningService(
        ApplicationDbContext context,
        ISystemSettingService settingService,
        ILogger<FullTimeTemplateProvisioningService> logger)
    {
        _context = context;
        _settingService = settingService;
        _logger = logger;
    }

    public async Task<int> EnsureSystemGeneratedTemplatesAsync(
        Ophthalmologist ophthalmologist,
        CancellationToken cancellationToken = default)
    {
        if (ophthalmologist.EmploymentType != OphthalmologistEmploymentType.FullTime)
        {
            return 0;
        }

        var minCost = await GetConfiguredCostAsync(MinSlotCostSettingKey, DefaultMinSlotCost, cancellationToken);
        var maxCost = await GetConfiguredCostAsync(MaxSlotCostSettingKey, DefaultMaxSlotCost, cancellationToken);

        if (minCost > maxCost)
        {
            (minCost, maxCost) = (maxCost, minCost);
        }

        var baseCost = ophthalmologist.YearsOfExperience * 50000m;
        var slotCost = decimal.Round(decimal.Clamp(baseCost, minCost, maxCost), 0, MidpointRounding.AwayFromZero);

        var createdCount = 0;
        var source = ScheduleTemplateSource.SystemGenerated.ToString();
        Guid? orgId = null;

        foreach (var day in DefaultWeekdays)
        {
            var insertedRows = await _context.Database.ExecuteSqlInterpolatedAsync(
                   $@"INSERT INTO ""ScheduleTemplates""
                     (""Id"", ""CreatedAt"", ""IsDeleted"", ""IsActive"", ""OrgId"", ""OphthalId"", ""DayOfWeek"", ""StartTime"", ""EndTime"", ""SlotDuration"", ""MaxCapacity"", ""Cost"", ""Source"")
                 VALUES ({Guid.NewGuid()}, {DateTime.UtcNow}, {false}, {true}, {orgId}, {ophthalmologist.Id}, {day.ToString()}, {DefaultStartTime}, {DefaultEndTime}, {DefaultSlotDurationMinutes}, {DefaultMaxCapacity}, {slotCost}, {source})
                     ON CONFLICT (""OphthalId"", ""DayOfWeek"")
                     WHERE ""IsDeleted"" = false
                       AND ""IsActive"" = true
                       AND ""Source"" = 'SystemGenerated'
                       AND ""OphthalId"" IS NOT NULL
                     DO NOTHING",
                cancellationToken);

            if (insertedRows > 0)
            {
                createdCount += insertedRows;
            }
        }

        if (createdCount > 0)
        {
            _logger.LogInformation(
                "Provisioned {CreatedCount} full-time system templates for ophthalmologist {OphthalmologistId}",
                createdCount,
                ophthalmologist.Id);
        }

        return createdCount;
    }

    private async Task<decimal> GetConfiguredCostAsync(
        string key,
        decimal fallback,
        CancellationToken cancellationToken)
    {
        var raw = await _settingService.GetSettingAsync(key, cancellationToken);
        if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed)
            && parsed >= 0)
        {
            return parsed;
        }

        return fallback;
    }
}
