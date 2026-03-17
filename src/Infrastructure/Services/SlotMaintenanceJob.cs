using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Hangfire recurring job for slot maintenance tasks.
/// Marks past, unused slots as Expired.
/// </summary>
public class SlotMaintenanceJob
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SlotMaintenanceJob> _logger;

    public SlotMaintenanceJob(ApplicationDbContext context, ILogger<SlotMaintenanceJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Expire slots whose start time has already passed and were never booked.
    /// Safe for concurrent workers because update is idempotent and predicate-guarded.
    /// </summary>
    public async Task ExpireUnusedSlotsAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        var utcDate = DateOnly.FromDateTime(utcNow);
        var utcTime = TimeOnly.FromDateTime(utcNow);

        _logger.LogInformation(
            "Starting slot expiration maintenance at {UtcNow}. Date={UtcDate}, Time={UtcTime}",
            utcNow,
            utcDate,
            utcTime);

        var expiredCount = await _context.AppointmentSlots
            .Where(slot =>
                slot.Status == ScheduleStatus.Available &&
                slot.BookedCount == 0 &&
                (slot.Date < utcDate || (slot.Date == utcDate && slot.StartTime < utcTime)))
            .ExecuteUpdateAsync(updates => updates
                .SetProperty(slot => slot.Status, ScheduleStatus.Expired)
                .SetProperty(slot => slot.UpdatedAt, utcNow),
                cancellationToken);

        _logger.LogInformation(
            "Slot expiration maintenance completed. Expired {ExpiredCount} slot(s).",
            expiredCount);
    }
}
