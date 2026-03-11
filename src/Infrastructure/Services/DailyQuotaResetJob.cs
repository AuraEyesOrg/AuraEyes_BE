using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Hangfire recurring job that resets daily AI screening quota.
/// Runs at 00:00 UTC (07:00 AM Vietnam time).
/// Uses ExecuteUpdateAsync for bulk update without loading entities.
/// </summary>
public class DailyQuotaResetJob
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DailyQuotaResetJob> _logger;

    public DailyQuotaResetJob(ApplicationDbContext context, ILogger<DailyQuotaResetJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting daily AI quota reset job at {Time} UTC", DateTime.UtcNow);

        try
        {
            // Reset active screenings to inactive (this resets the used quota counter)
            // AiQuotaService counts active screenings as UsedQuota,
            // so deactivating them effectively resets UsedAiQuota = 0
            var resetCount = await _context.AiScreenings
                .Where(s => s.IsActive)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.IsActive, false)
                    .SetProperty(s => s.UpdatedAt, DateTime.UtcNow));

            _logger.LogInformation(
                "Daily quota reset completed. Deactivated {Count} AI screenings", resetCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute daily quota reset job");
            throw;
        }
    }
}
