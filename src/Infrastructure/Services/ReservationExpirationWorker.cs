using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Background worker that periodically checks for expired slot reservations
/// and releases them back to available status.
/// </summary>
public class ReservationExpirationWorker : BackgroundService
{
    /// <summary>How often to check for expired reservations.</summary>
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IBetterStackHeartbeatService _betterStackHeartbeat;
    private readonly ILogger<ReservationExpirationWorker> _logger;

    public ReservationExpirationWorker(
        IServiceScopeFactory scopeFactory,
        IBetterStackHeartbeatService betterStackHeartbeat,
        ILogger<ReservationExpirationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _betterStackHeartbeat = betterStackHeartbeat;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReservationExpirationWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _betterStackHeartbeat.NotifyStartedAsync(BetterStackMonitor.ReservationExpirationWorker, stoppingToken);
                await ReleaseExpiredReservationsAsync(stoppingToken);
                await _betterStackHeartbeat.NotifySucceededAsync(BetterStackMonitor.ReservationExpirationWorker, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                await _betterStackHeartbeat.NotifyFailedAsync(BetterStackMonitor.ReservationExpirationWorker, stoppingToken);
                _logger.LogError(ex, "Error in ReservationExpirationWorker cycle");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }

        _logger.LogInformation("ReservationExpirationWorker stopped");
    }

    private async Task ReleaseExpiredReservationsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var slotRepo = scope.ServiceProvider.GetRequiredService<IAppointmentSlotRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var expiredSlots = await slotRepo.GetExpiredReservationsAsync(cancellationToken);

        if (expiredSlots.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Found {Count} expired reservation(s) to release", expiredSlots.Count);

        var releasedCount = 0;

        foreach (var slot in expiredSlots)
        {
            try
            {
                var patientId = slot.ReservedBy;
                slot.ReleaseReservation();

                await slotRepo.UpdateAsync(slot, cancellationToken);
                releasedCount++;

                _logger.LogDebug(
                    "Released expired reservation for slot {SlotId} (was reserved by {PatientId})",
                    slot.Id, patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to release expired reservation for slot {SlotId}",
                    slot.Id);
            }
        }

        if (releasedCount > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Released {Count} expired reservation(s)", releasedCount);
        }
    }
}
