using Application.Common.Interfaces;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Background worker that periodically checks for stale consultation sessions
/// and sends reminders to the assigned doctors.
/// A 48-hour cooldown prevents duplicate notifications for the same session.
/// </summary>
public class SessionReminderWorker : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(6);
    private static readonly TimeSpan InactivityThreshold = TimeSpan.FromDays(3);
    private static readonly TimeSpan ReminderCooldown = TimeSpan.FromHours(48);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SessionReminderWorker> _logger;

    public SessionReminderWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<SessionReminderWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SessionReminderWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndNotifyStaleSessionsAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error in SessionReminderWorker cycle");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }

        _logger.LogInformation("SessionReminderWorker stopped");
    }

    private async Task CheckAndNotifyStaleSessionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var sessionRepo = scope.ServiceProvider.GetRequiredService<IConsultationSessionRepository>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var staleSessions = await sessionRepo.GetStaleSessions(
            InactivityThreshold, ReminderCooldown, cancellationToken);

        if (staleSessions.Count == 0)
        {
            _logger.LogDebug("No stale sessions require reminders");
            return;
        }

        _logger.LogInformation("Found {Count} stale session(s) eligible for reminders", staleSessions.Count);

        var remindersSent = 0;

        foreach (var session in staleSessions)
        {
            if (!session.OphthalmologistId.HasValue)
            {
                _logger.LogWarning(
                    "Stale session {SessionId} has no assigned doctor, skipping reminder",
                    session.Id);
                continue;
            }

            var daysSinceActivity = (DateTime.UtcNow - session.LastActivityAt).Days;

            await notificationService.SendAsync(
                session.OphthalmologistId.Value,
                $"Reminder: Session #{session.Id} has been inactive for {daysSinceActivity} day(s). " +
                "Please review or end the session.",
                cancellationToken);

            session.RecordReminderSent();
            remindersSent++;

            _logger.LogInformation(
                "Sent stale session reminder for {SessionId} to doctor {DoctorId}",
                session.Id, session.OphthalmologistId.Value);
        }

        if (remindersSent > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
