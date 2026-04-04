using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Hangfire recurring job that resets daily AI screening quota.
/// Runs at 00:00 UTC (07:00 AM Vietnam time).
/// Resets UsedAiQuota = 0 on Patients and Organisations using bulk ExecuteUpdateAsync.
/// Does NOT touch AiScreenings — screening history is preserved.
/// </summary>
public class DailyQuotaResetJob
{
    private readonly ApplicationDbContext _context;
    private readonly IBetterStackHeartbeatService _betterStackHeartbeat;
    private readonly ILogger<DailyQuotaResetJob> _logger;

    public DailyQuotaResetJob(
        ApplicationDbContext context,
        IBetterStackHeartbeatService betterStackHeartbeat,
        ILogger<DailyQuotaResetJob> logger)
    {
        _context = context;
        _betterStackHeartbeat = betterStackHeartbeat;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting daily AI quota reset job at {Time} UTC", DateTime.UtcNow);
        await _betterStackHeartbeat.NotifyStartedAsync(BetterStackMonitor.DailyQuotaReset);

        try
        {
            var patientResetCount = await _context.Patients
                .Where(p => p.UsedAiQuota > 0)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.UsedAiQuota, 0));

            var orgResetCount = await _context.Organisations
                .Where(o => o.UsedAiQuota > 0)
                .ExecuteUpdateAsync(s => s.SetProperty(o => o.UsedAiQuota, 0));

            _logger.LogInformation(
                "Daily quota reset completed. Reset {PatientCount} patients, {OrgCount} organisations",
                patientResetCount, orgResetCount);

            await _betterStackHeartbeat.NotifySucceededAsync(BetterStackMonitor.DailyQuotaReset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute daily quota reset job");
            await _betterStackHeartbeat.NotifyFailedAsync(BetterStackMonitor.DailyQuotaReset);
            throw;
        }
    }
}
