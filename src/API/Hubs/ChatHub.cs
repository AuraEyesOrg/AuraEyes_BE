using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

/// <summary>
/// SignalR Hub for realtime consultation chat events.
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<Ophthalmologist> _ophthalmologistRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IChatHubService _chatHubService;

    public ChatHub(
        ILogger<ChatHub> logger,
        IConsultationSessionRepository sessionRepository,
        IRepository<Ophthalmologist> ophthalmologistRepository,
        IRepository<Patient> patientRepository,
        IChatHubService chatHubService)
    {
        _logger = logger;
        _sessionRepository = sessionRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _patientRepository = patientRepository;
        _chatHubService = chatHubService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation(
            "ChatHub client connected: ConnectionId={ConnectionId}, UserId={UserId}",
            Context.ConnectionId,
            userId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation(
            "ChatHub client disconnected: ConnectionId={ConnectionId}, UserId={UserId}",
            Context.ConnectionId,
            userId);

        if (exception != null)
        {
            _logger.LogWarning(exception, "ChatHub client disconnected with error");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Broadcasts the sender typing state to the other participant in a consultation session.
    /// </summary>
    public async Task SendTyping(Guid sessionId, bool isTyping)
    {
        var senderUserIdRaw = Context.UserIdentifier;
        var senderProfileIdRaw = Context.User?.FindFirst("profile_id")?.Value;

        if (!Guid.TryParse(senderUserIdRaw, out var senderUserId)
            || !Guid.TryParse(senderProfileIdRaw, out var senderProfileId))
        {
            _logger.LogDebug(
                "Ignoring typing event due to invalid sender identity. ConnectionId={ConnectionId}",
                Context.ConnectionId);
            return;
        }

        var session = await _sessionRepository.GetByIdAsync(sessionId, Context.ConnectionAborted);
        if (session is null)
        {
            _logger.LogDebug(
                "Ignoring typing event for unknown SessionId={SessionId}",
                sessionId);
            return;
        }

        bool isPatient = senderProfileId == session.PatientId;
        bool isDoctor = session.OphthalmologistId.HasValue
                && senderProfileId == session.OphthalmologistId.Value;

        if (!isPatient && !isDoctor)
        {
            _logger.LogWarning(
                "Ignoring typing event from non-participant ProfileId={ProfileId} for SessionId={SessionId}",
                senderProfileId,
                sessionId);
            return;
        }

        if (session.ChatStatus == ChatStatus.Locked || session.ChatStatus == ChatStatus.Archived)
        {
            return;
        }

        if (session.ChatStatus == ChatStatus.MemoOnly && isDoctor)
        {
            return;
        }

        Guid? recipientUserId = null;

        if (isPatient && session.OphthalmologistId.HasValue)
        {
            var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(
                session.OphthalmologistId.Value,
                Context.ConnectionAborted);
            recipientUserId = ophthalmologist?.UserId;
        }
        else if (isDoctor)
        {
            var patient = await _patientRepository.GetByIdAsync(
                session.PatientId,
                Context.ConnectionAborted);
            recipientUserId = patient?.UserId;
        }

        if (!recipientUserId.HasValue || recipientUserId.Value == senderUserId)
        {
            return;
        }

        await _chatHubService.BroadcastTypingIndicatorAsync(
            recipientUserId.Value,
            new TypingIndicatorRealtimeDto
            {
                SessionId = sessionId,
                SenderProfileId = senderProfileId,
                IsTyping = isTyping,
                Timestamp = DateTime.UtcNow
            },
            Context.ConnectionAborted);
    }
}
