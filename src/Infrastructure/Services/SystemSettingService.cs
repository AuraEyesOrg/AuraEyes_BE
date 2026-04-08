using Application.Common.Constants;
using Application.SystemSettings.Interfaces;
using Domain.Entities.Platform;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Infrastructure.Services;

public class SystemSettingService : ISystemSettingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SystemSettingService> _logger;

    public SystemSettingService(ApplicationDbContext context, ILogger<SystemSettingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string?> GetSettingAsync(string key, CancellationToken cancellationToken = default)
    {
        var setting = await _context.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);

        return setting?.Value;
    }

    public async Task<Dictionary<string, string>> GetAllSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settingsList = await _context.SystemSettings
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return settingsList.ToDictionary(s => s.Key, s => s.Value);
    }

    public async Task UpdateSettingsAsync(Dictionary<string, string> settings, CancellationToken cancellationToken = default)
    {
        if (settings == null || !settings.Any()) return;

        var keys = settings.Keys.ToList();
        var existingSettings = await _context.SystemSettings
            .Where(s => keys.Contains(s.Key))
            .ToDictionaryAsync(s => s.Key, cancellationToken);

        foreach (var kvp in settings)
        {
            if (existingSettings.TryGetValue(kvp.Key, out var existing))
            {
                existing.UpdateValue(kvp.Value);
            }
            else
            {
                var newSetting = new SystemSetting(kvp.Key, kvp.Value);
                await _context.SystemSettings.AddAsync(newSetting, cancellationToken);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("System settings updated for keys: {Keys}", string.Join(", ", keys));
    }

    public async Task<(bool Success, int UsedSlots, int Quota, int RemainingSlots)> TryReservePartTimeSlotsAsync(
        DateOnly date,
        int slotsToReserve,
        int quotaSnapshot,
        CancellationToken cancellationToken = default)
    {
        if (slotsToReserve < 1)
            throw new ArgumentOutOfRangeException(nameof(slotsToReserve), "Reserved slots must be at least 1.");

        if (quotaSnapshot < 1)
            throw new ArgumentOutOfRangeException(nameof(quotaSnapshot), "Quota must be at least 1.");

        var counterKey = BuildPartTimeReservedSlotsKey(date);
        var description = $"Reserved part-time slots for {date:yyyy-MM-dd}";

        await _context.Database.ExecuteSqlInterpolatedAsync(
            $@"INSERT INTO ""SystemSettings"" (""Key"", ""Value"", ""Description"")
               VALUES ({counterKey}, {"0"}, {description})
               ON CONFLICT (""Key"") DO NOTHING",
            cancellationToken);

        var setting = await _context.SystemSettings
            .FromSqlInterpolated($@"SELECT * FROM ""SystemSettings"" WHERE ""Key"" = {counterKey} FOR UPDATE")
            .SingleAsync(cancellationToken);

        var currentUsed = TryParseNonNegativeInt(setting.Value);
        var proposed = currentUsed + slotsToReserve;

        if (proposed > quotaSnapshot)
        {
            return (false, currentUsed, quotaSnapshot, Math.Max(0, quotaSnapshot - currentUsed));
        }

        setting.UpdateValue(proposed.ToString(CultureInfo.InvariantCulture));

        return (true, proposed, quotaSnapshot, Math.Max(0, quotaSnapshot - proposed));
    }

    public async Task<IReadOnlyDictionary<DateOnly, int>> GetPartTimeReservedSlotsByDateRangeAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        var prefix = SystemSettingKeys.PartTimeReservedSlotsPrefix;

        var rows = await _context.SystemSettings
            .AsNoTracking()
            .Where(s => EF.Functions.Like(s.Key, $"{prefix}%"))
            .ToListAsync(cancellationToken);

        var result = new Dictionary<DateOnly, int>();

        foreach (var row in rows)
        {
            if (!TryParseDateFromPartTimeReservedSlotsKey(row.Key, out var date))
                continue;

            if (date < fromDate || date > toDate)
                continue;

            result[date] = TryParseNonNegativeInt(row.Value);
        }

        return result;
    }

    private static string BuildPartTimeReservedSlotsKey(DateOnly date)
        => $"{SystemSettingKeys.PartTimeReservedSlotsPrefix}{date:yyyyMMdd}";

    private static bool TryParseDateFromPartTimeReservedSlotsKey(string key, out DateOnly date)
    {
        var prefix = SystemSettingKeys.PartTimeReservedSlotsPrefix;
        date = default;

        if (!key.StartsWith(prefix, StringComparison.Ordinal))
            return false;

        var suffix = key[prefix.Length..];
        return DateOnly.TryParseExact(suffix, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    private static int TryParseNonNegativeInt(string value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) && parsed >= 0
            ? parsed
            : 0;
    }
}
