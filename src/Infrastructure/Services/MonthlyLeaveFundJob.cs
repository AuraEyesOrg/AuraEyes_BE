using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Hangfire recurring job to increment leave fund for active staff members.
/// Runs on the 1st of every month at 00:00.
/// </summary>
public class MonthlyLeaveFundJob
{
    private readonly ApplicationDbContext _context;
    private readonly IBetterStackHeartbeatService _betterStackHeartbeat;
    private readonly ILogger<MonthlyLeaveFundJob> _logger;

    public MonthlyLeaveFundJob(
        ApplicationDbContext context,
        IBetterStackHeartbeatService betterStackHeartbeat,
        ILogger<MonthlyLeaveFundJob> logger)
    {
        _context = context;
        _betterStackHeartbeat = betterStackHeartbeat;
        _logger = logger;
    }

    /// <summary>
    /// Increments AvailableLeaveDays by 1 for all active Ophthalmologists and ClinicStaffs.
    /// Uses ExecuteUpdateAsync for a single high-performance SQL query per table.
    /// </summary>
    public async Task IncrementMonthlyLeaveDaysAsync(CancellationToken cancellationToken = default)
    {
        // Notifying heartbeat if applicable (using a generic or new monitor enum if exists)
        // For now, we'll just log and execute.
        
        _logger.LogInformation("Starting monthly leave fund increment job at {Time}", DateTime.UtcNow);

        try
        {
            // 1. Update Ophthalmologists
            // Condition: Linked User must be Active and Not Deleted.
            int doctorUpdates = await _context.Ophthalmologists
                .Where(o => _context.Users.Any(u => u.Id == o.UserId && u.IsActive && !u.IsDeleted))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(o => o.AvailableLeaveDays, o => o.AvailableLeaveDays + 1)
                    .SetProperty(o => o.UpdatedAt, DateTime.UtcNow),
                    cancellationToken);

            

            _logger.LogInformation(
                "Monthly leave fund increment completed. Updated {DoctorCount} doctors.",
                doctorUpdates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during monthly leave fund increment job.");
            throw;
        }
    }
}
