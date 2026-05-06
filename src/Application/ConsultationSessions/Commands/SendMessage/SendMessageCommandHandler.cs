using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ConsultationSessions.Commands.SendMessage;

public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IRepository<Ophthalmologist> _ophthalmologistRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly INotificationService _notificationService;
    private readonly IChatHubService _chatHubService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SendMessageCommandHandler> _logger;

    public SendMessageCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IRepository<Conversation> conversationRepository,
        ICurrentUserService currentUser,
        IRepository<Ophthalmologist> ophthalmologistRepository,
        IRepository<Patient> patientRepository,
        INotificationService notificationService,
        IChatHubService chatHubService,
        IUnitOfWork unitOfWork,
        ILogger<SendMessageCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _conversationRepository = conversationRepository;
        _currentUser = currentUser;
        _ophthalmologistRepository = ophthalmologistRepository;
        _patientRepository = patientRepository;
        _notificationService = notificationService;
        _chatHubService = chatHubService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.ProfileId.HasValue)
            return Result.Unauthorized("Authenticated profile is required to send messages.");

        var senderProfileId = _currentUser.ProfileId.Value;

        var session = await _sessionRepository.GetByIdWithConversationsForUpdateAsync(
            request.SessionId, cancellationToken);

        if (session is null)
            return Result.NotFound($"Session '{request.SessionId}' not found.");

        var validationResult = ValidateSendMessage(session, senderProfileId, out bool isPatient, out bool isDoctor);
        if (!validationResult.IsSuccess)
            return validationResult;

        var conversation = session.Conversations.FirstOrDefault();
        if (conversation is null)
        {
            if (!session.OphthalmologistId.HasValue)
                return Result.Failure("No ophthalmologist assigned to this session yet.");

            conversation = new Conversation(session.OphthalmologistId.Value, request.SessionId);
            await _conversationRepository.AddAsync(conversation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var chatMessage = new ChatMessage(conversation.Id, senderProfileId, request.Message);
        conversation.AddMessage(chatMessage);

        session.RecordActivity();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Persisted chat message {MessageId} in session {SessionId} from profile {SenderProfileId}",
            chatMessage.Id,
            session.Id,
            senderProfileId);

        if (session.ChatStatus == ChatStatus.MemoOnly)
            return Result.Success();

        await BroadcastAndNotifyAsync(session, chatMessage, senderProfileId, isPatient, isDoctor, cancellationToken);

        return Result.Success();
    }

    private static Result ValidateSendMessage(
        Domain.Entities.Consultation.ConsultationSession session,
        Guid senderProfileId,
        out bool isPatient,
        out bool isDoctor)
    {
        isPatient = senderProfileId == session.PatientId;
        isDoctor = session.OphthalmologistId.HasValue && senderProfileId == session.OphthalmologistId.Value;

        if (!isPatient && !isDoctor)
            return Result.Forbidden("You are not a participant of this session.");

        if (session.ChatStatus == ChatStatus.Locked)
            return Result.Failure("Chat is locked for this session.");

        if (session.ChatStatus == ChatStatus.Archived)
            return Result.Failure("Session has been archived. No new messages allowed.");

        if (session.EndTime.HasValue && DateTime.UtcNow > session.EndTime.Value.AddDays(7))
            return Result.Failure("Chat is locked as the 7-day grace period after consultation has expired.");

        if (session.ChatStatus == ChatStatus.MemoOnly && isDoctor)
            return Result.Failure("In MemoOnly mode, only the patient can send notes.");

        return Result.Success();
    }

    private async Task BroadcastAndNotifyAsync(
        Domain.Entities.Consultation.ConsultationSession session,
        ChatMessage chatMessage,
        Guid senderProfileId,
        bool isPatient,
        bool isDoctor,
        CancellationToken cancellationToken)
    {
        Guid? recipientUserId = null;

        if (isPatient && session.OphthalmologistId.HasValue)
        {
            var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(
                session.OphthalmologistId.Value,
                cancellationToken);
            recipientUserId = ophthalmologist?.UserId;
        }
        else if (isDoctor)
        {
            var patient = await _patientRepository.GetByIdAsync(
                session.PatientId,
                cancellationToken);
            recipientUserId = patient?.UserId;
        }

        var participants = new List<Guid>();
        if (_currentUser.UserId.HasValue) participants.Add(_currentUser.UserId.Value);
        if (recipientUserId.HasValue) participants.Add(recipientUserId.Value);
        var senderUserId = _currentUser.UserId;

        var distinctParticipants = participants.Distinct().ToList();

        _logger.LogInformation(
            "Dispatching realtime chat message {MessageId} for session {SessionId} to {ParticipantCount} participants. RecipientUserId={RecipientUserId}",
            chatMessage.Id,
            session.Id,
            distinctParticipants.Count,
            recipientUserId);

        if (!recipientUserId.HasValue)
        {
            _logger.LogWarning(
                "No recipient user resolved for session {SessionId} and sender profile {SenderProfileId}. Realtime dispatch will only include sender user when available.",
                session.Id,
                senderProfileId);
        }

        foreach (var userId in distinctParticipants)
        {
            var targetProfileId = senderUserId.HasValue && userId == senderUserId.Value
                ? senderProfileId
                : isDoctor
                    ? session.PatientId
                    : session.OphthalmologistId ?? session.PatientId;

            await _chatHubService.BroadcastChatMessageAsync(
                userId,
                targetProfileId,
                new ChatMessageRealtimeDto
                {
                    SessionId = session.Id,
                    MessageId = chatMessage.Id,
                    SenderProfileId = senderProfileId,
                    Content = chatMessage.Message,
                    SentAt = chatMessage.SentAt
                },
                cancellationToken);
        }

        if (isPatient && session.OphthalmologistId.HasValue)
        {
            var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(
                session.OphthalmologistId.Value,
                cancellationToken);

            if (ophthalmologist is not null)
            {
                var messagePreview = chatMessage.Message.Length > 50
                    ? chatMessage.Message[..50] + "..."
                    : chatMessage.Message;

                await _notificationService.SendAsync(
                    ophthalmologist.UserId,
                    "Tin nhắn mới từ bệnh nhân",
                    $"Bạn có tin nhắn mới: \"{messagePreview}\"",
                    NotificationType.NewPatientMessage,
                    new { ConsultationId = session.Id, PatientId = session.PatientId },
                    cancellationToken);
            }
        }
    }
}
