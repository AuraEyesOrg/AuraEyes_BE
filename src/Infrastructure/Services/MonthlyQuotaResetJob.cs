using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Backward-compatible Hangfire job kept for previously persisted job payloads.
/// Delegates execution to <see cref="DailyQuotaResetJob"/>.
/// </summary>
[Obsolete("Use DailyQuotaResetJob instead. This class exists for Hangfire backward compatibility.")]
public class MonthlyQuotaResetJob
{
    private readonly DailyQuotaResetJob _dailyQuotaResetJob;
    private readonly ILogger<MonthlyQuotaResetJob> _logger;

    public MonthlyQuotaResetJob(
        DailyQuotaResetJob dailyQuotaResetJob,
        ILogger<MonthlyQuotaResetJob> logger)
    {
        _dailyQuotaResetJob = dailyQuotaResetJob;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogWarning(
            "Executing legacy MonthlyQuotaResetJob for backward compatibility. Delegating to DailyQuotaResetJob.");

        await _dailyQuotaResetJob.ExecuteAsync();
    }
}