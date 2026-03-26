using Application.SystemSettings.Interfaces;
using Domain.Entities.Platform;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
}
