using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ConsultationSessions.Commands.EndSession;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;
using Infrastructure.Settings;
using MediatR;
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
    private readonly IBetterStackHeartbeatService _betterStackHeartbeat;
    private readonly ILogger<ConsultationStateWorker> _logger;
    private readonly TimeSpan _slotDuration;

    public ConsultationStateWorker(
        IServiceScopeFactory scopeFactory,
        IBetterStackHeartbeatService betterStackHeartbeat,
        IOptions<GoogleMeetSettings> googleMeetSettings,
        ILogger<ConsultationStateWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _betterStackHeartbeat = betterStackHeartbeat;
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
                await _betterStackHeartbeat.NotifyStartedAsync(BetterStackMonitor.ConsultationStateWorker, stoppingToken);
                await ProcessStateTransitionsAsync(stoppingToken);
                await _betterStackHeartbeat.NotifySucceededAsync(BetterStackMonitor.ConsultationStateWorker, stoppingToken);
            }
            catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("ConsultationStateWorker cycle was canceled by infrastructure timeout and will retry.");
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                await _betterStackHeartbeat.NotifyFailedAsync(BetterStackMonitor.ConsultationStateWorker, stoppingToken);
                _logger.LogError(ex, "Error in ConsultationStateWorker cycle");
            }

            try
            {
                await Task.Delay(CheckInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
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
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await OpenReadySessionsAsync(sessionRepo, patientRepo, ophthalmologistRepo, chatHubService, unitOfWork, cancellationToken);
        await ArchiveExpiredSessionsAsync(
            sessionRepo,
            patientRepo,
            ophthalmologistRepo,
            chatHubService,
            sender,
            unitOfWork,
            cancellationToken);

        await ArchiveExpiredClinicSessionsAsync(
            sessionRepo,
            patientRepo,
            ophthalmologistRepo,
            chatHubService,
            sender,
            unitOfWork,
            cancellationToken);
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
        ISender sender,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var sessions = await sessionRepo.GetSessionsPastGracePeriodAsync(
            _slotDuration, GracePeriod, cancellationToken);

        if (sessions.Count == 0) return;

        _logger.LogInformation("Auto-completing {Count} session(s) past grace period", sessions.Count);

        var closedSessions = new List<(Guid SessionId, Guid PatientId, Guid? OphthalmologistId)>();
        var closedBySystemCount = 0;

        foreach (var session in sessions)
        {
            if (!session.OphthalmologistId.HasValue)
            {
                session.CompleteBySystem();
                await sessionRepo.UpdateAsync(session, cancellationToken);
                closedSessions.Add((session.Id, session.PatientId, session.OphthalmologistId));
                closedBySystemCount++;

                _logger.LogWarning(
                    "Session {SessionId} has no assigned doctor. Closed by system without payout.",
                    session.Id);

                continue;
            }

            var result = await sender.Send(
                new EndSessionCommand
                {
                    SessionId = session.Id,
                    DoctorId = session.OphthalmologistId.Value,
                    Reason = "GracePeriodExpired"
                },
                cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "Failed to auto-complete session {SessionId}: {Error}",
                    session.Id,
                    result.ErrorMessage);
                continue;
            }

            closedSessions.Add((session.Id, session.PatientId, session.OphthalmologistId));
        }

        if (closedBySystemCount > 0)
            await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var session in closedSessions)
        {
            var userIds = await ResolveParticipantUserIdsAsync(
                session.PatientId, session.OphthalmologistId,
                patientRepo, ophthalmologistRepo, cancellationToken);

            await chatHubService.BroadcastRoomStateChangedAsync(
                userIds,
                new RoomStateChangedDto
                {
                    SessionId = session.SessionId,
                    Event = "ROOM_CLOSED",
                    Timestamp = DateTime.UtcNow
                },
                cancellationToken);

            _logger.LogInformation(
                "ROOM_CLOSED for session {SessionId} (grace period expired)",
                session.SessionId);
        }
    }

    private async Task ArchiveExpiredClinicSessionsAsync(
        IConsultationSessionRepository sessionRepo,
        IRepository<Patient> patientRepo,
        IRepository<Ophthalmologist> ophthalmologistRepo,
        IChatHubService chatHubService,
        ISender sender,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var threshold = TimeSpan.FromDays(14);
        var sessions = await sessionRepo.GetExpiredClinicSessionsAsync(threshold, cancellationToken);

        if (sessions.Count == 0) return;

        _logger.LogInformation("Auto-completing {Count} clinic session(s) past 14-day window", sessions.Count);

        var closedSessions = new List<(Guid SessionId, Guid PatientId, Guid? OphthalmologistId)>();

        foreach (var session in sessions)
        {
            if (!session.OphthalmologistId.HasValue)
            {
                session.CompleteBySystem("ClinicFollowupWindowExpired");
                await sessionRepo.UpdateAsync(session, cancellationToken);
                closedSessions.Add((session.Id, session.PatientId, session.OphthalmologistId));
                continue;
            }

            var result = await sender.Send(
                new EndSessionCommand
                {
                    SessionId = session.Id,
                    DoctorId = session.OphthalmologistId.Value,
                    Reason = "ClinicFollowupWindowExpired"
                },
                cancellationToken);

            if (result.IsSuccess)
            {
                closedSessions.Add((session.Id, session.PatientId, session.OphthalmologistId));
            }
        }

        foreach (var session in closedSessions)
        {
            var userIds = await ResolveParticipantUserIdsAsync(
                session.PatientId, session.OphthalmologistId,
                patientRepo, ophthalmologistRepo, cancellationToken);

            await chatHubService.BroadcastRoomStateChangedAsync(
                userIds,
                new RoomStateChangedDto
                {
                    SessionId = session.SessionId,
                    Event = "ROOM_CLOSED",
                    Timestamp = DateTime.UtcNow
                },
                cancellationToken);
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
        if (patient is not null && patient.UserId.HasValue)
            userIds.Add(patient.UserId.Value);

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
