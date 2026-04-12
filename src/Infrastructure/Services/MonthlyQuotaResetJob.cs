using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Hangfire recurring job that resets organisation monthly quota usage.
/// Runs once per month and does not roll over unused quota.
/// </summary>
public class MonthlyQuotaResetJob
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MonthlyQuotaResetJob> _logger;

    public MonthlyQuotaResetJob(ApplicationDbContext context, ILogger<MonthlyQuotaResetJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var now = DateTime.UtcNow;
        _logger.LogInformation("Starting monthly AI quota reset job at {Time} UTC", now);

        try
        {
            var resetCount = await _context.Organisations
                .Where(o => o.MonthlyQuotaLimit > 0)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(o => o.MonthlyQuotaUsed, 0)
                    .SetProperty(o => o.MonthlyQuotaLastResetAt, now));

            _logger.LogInformation(
                "Monthly quota reset completed. Reset {OrganisationCount} organisations",
                resetCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute monthly quota reset job");
            throw;
        }
    }
}
