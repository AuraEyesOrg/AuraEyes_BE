using Application.Common.Helpers;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Hangfire recurring job for slot maintenance tasks.
/// Marks past slots as Blocked and clears expired reservations.
/// NOTE: AppointmentSlot.Date and StartTime are stored in Vietnam local time (UTC+7).
/// All comparisons MUST use local time, not UTC.
/// </summary>
public class SlotMaintenanceJob
{
    private readonly ApplicationDbContext _context;
    private readonly IBetterStackHeartbeatService _betterStackHeartbeat;
    private readonly ILogger<SlotMaintenanceJob> _logger;

    public SlotMaintenanceJob(
        ApplicationDbContext context,
        IBetterStackHeartbeatService betterStackHeartbeat,
        ILogger<SlotMaintenanceJob> logger)
    {
        _context = context;
        _betterStackHeartbeat = betterStackHeartbeat;
        _logger = logger;
    }

    /// <summary>
    /// Expire slots whose start time has already passed (in local Vietnam time)
    /// and clear expired temporary reservations.
    /// Safe for concurrent workers because update is idempotent and predicate-guarded.
    /// </summary>
    public async Task ExpireUnusedSlotsAsync(CancellationToken cancellationToken = default)
    {
        await _betterStackHeartbeat.NotifyStartedAsync(BetterStackMonitor.SlotMaintenance, cancellationToken);

        // Slot Date/StartTime are stored in Vietnam local time (UTC+7), so we must
        // compare against local time — NOT UTC — to correctly identify past slots.
        var utcNow = DateTime.UtcNow;
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, VietnamTimeZoneResolver.TimeZone);
        var localDate = DateOnly.FromDateTime(localNow);
        var localTime = TimeOnly.FromDateTime(localNow);

        try
        {
            _logger.LogInformation(
                "Starting slot expiration maintenance. UTC={UtcNow}, Local(VN)={LocalNow}, Date={LocalDate}, Time={LocalTime}",
                utcNow,
                localNow,
                localDate,
                localTime);

            // Block past slots (Date/StartTime stored in local VN time, compare with local time).
            var expiredCount = await _context.AppointmentSlots
                .Where(slot =>
                    slot.Status == ScheduleStatus.Available &&
                    (slot.Date < localDate ||
                     (slot.Date == localDate && slot.StartTime < localTime)))
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(slot => slot.Status, ScheduleStatus.Blocked)
                    .SetProperty(slot => slot.UpdatedAt, utcNow),
                    cancellationToken);

            _logger.LogInformation(
                "Slot expiration maintenance completed. Blocked {ExpiredCount} past slot(s).",
                expiredCount);

            await _betterStackHeartbeat.NotifySucceededAsync(BetterStackMonitor.SlotMaintenance, cancellationToken);
        }
        catch (Exception)
        {
            await _betterStackHeartbeat.NotifyFailedAsync(BetterStackMonitor.SlotMaintenance, cancellationToken);
            throw;
        }
    }
}
