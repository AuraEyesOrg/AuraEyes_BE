using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;
using Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

/// <summary>
/// Background worker that enforces time-based chat state transitions for VideoCall sessions:
///   MemoOnly → Open   at AppointmentTime         (broadcasts ROOM_OPENED)
///   Open     → Archived after grace period expires (broadcasts ROOM_CLOSED)
/// Runs every 15 seconds to keep transitions near-realtime.
/// </summary>
public class ConsultationStateWorker : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan GracePeriod = TimeSpan.FromHours(2);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ConsultationStateWorker> _logger;
    private readonly TimeSpan _slotDuration;

    public ConsultationStateWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<GoogleMeetSettings> googleMeetSettings,
        ILogger<ConsultationStateWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        var durationMinutes = Math.Max(1, googleMeetSettings.Value.DefaultDurationMinutes);
        _slotDuration = TimeSpan.FromMinutes(durationMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ConsultationStateWorker started (interval={Interval}s)", CheckInterval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessStateTransitionsAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error in ConsultationStateWorker cycle");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }

        _logger.LogInformation("ConsultationStateWorker stopped");
    }

    private async Task ProcessStateTransitionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var sessionRepo = scope.ServiceProvider.GetRequiredService<IConsultationSessionRepository>();
        var patientRepo = scope.ServiceProvider.GetRequiredService<IRepository<Patient>>();
        var ophthalmologistRepo = scope.ServiceProvider.GetRequiredService<IRepository<Ophthalmologist>>();
        var chatHubService = scope.ServiceProvider.GetRequiredService<IChatHubService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await OpenReadySessionsAsync(sessionRepo, patientRepo, ophthalmologistRepo, chatHubService, unitOfWork, cancellationToken);
        await ArchiveExpiredSessionsAsync(sessionRepo, patientRepo, ophthalmologistRepo, chatHubService, unitOfWork, cancellationToken);
    }

    private async Task OpenReadySessionsAsync(
        IConsultationSessionRepository sessionRepo,
        IRepository<Patient> patientRepo,
        IRepository<Ophthalmologist> ophthalmologistRepo,
        IChatHubService chatHubService,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var sessions = await sessionRepo.GetSessionsReadyToOpenAsync(cancellationToken);
        if (sessions.Count == 0) return;

        _logger.LogInformation("Opening chat for {Count} session(s) at appointment time", sessions.Count);

        foreach (var session in sessions)
        {
            session.OpenChat();
            await sessionRepo.UpdateAsync(session, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var session in sessions)
        {
            var userIds = await ResolveParticipantUserIdsAsync(
                session.PatientId, session.OphthalmologistId,
                patientRepo, ophthalmologistRepo, cancellationToken);

            await chatHubService.BroadcastRoomStateChangedAsync(
                userIds,
                new RoomStateChangedDto
                {
                    SessionId = session.Id,
                    Event = "ROOM_OPENED",
                    Timestamp = DateTime.UtcNow
                },
                cancellationToken);

            _logger.LogInformation(
                "ROOM_OPENED for session {SessionId} (appointment {AppointmentTime})",
                session.Id, session.AppointmentTime);
        }
    }

    private async Task ArchiveExpiredSessionsAsync(
        IConsultationSessionRepository sessionRepo,
        IRepository<Patient> patientRepo,
        IRepository<Ophthalmologist> ophthalmologistRepo,
        IChatHubService chatHubService,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var sessions = await sessionRepo.GetSessionsPastGracePeriodAsync(
            _slotDuration, GracePeriod, cancellationToken);

        if (sessions.Count == 0) return;

        _logger.LogInformation("Archiving {Count} session(s) past grace period", sessions.Count);

        foreach (var session in sessions)
        {
            session.CompleteBySystem();
            await sessionRepo.UpdateAsync(session, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var session in sessions)
        {
            var userIds = await ResolveParticipantUserIdsAsync(
                session.PatientId, session.OphthalmologistId,
                patientRepo, ophthalmologistRepo, cancellationToken);

            await chatHubService.BroadcastRoomStateChangedAsync(
                userIds,
                new RoomStateChangedDto
                {
                    SessionId = session.Id,
                    Event = "ROOM_CLOSED",
                    Timestamp = DateTime.UtcNow
                },
                cancellationToken);

            _logger.LogInformation(
                "ROOM_CLOSED for session {SessionId} (grace period expired)",
                session.Id);
        }
    }

    private static async Task<List<Guid>> ResolveParticipantUserIdsAsync(
        Guid patientId,
        Guid? ophthalmologistId,
        IRepository<Patient> patientRepo,
        IRepository<Ophthalmologist> ophthalmologistRepo,
        CancellationToken cancellationToken)
    {
        var userIds = new List<Guid>(2);

        var patient = await patientRepo.GetByIdAsync(patientId, cancellationToken);
        if (patient is not null)
            userIds.Add(patient.UserId);

        if (ophthalmologistId.HasValue)
        {
            var doctor = await ophthalmologistRepo.GetByIdAsync(
                ophthalmologistId.Value, cancellationToken);
            if (doctor is not null)
                userIds.Add(doctor.UserId);
        }

        return userIds;
    }
}
