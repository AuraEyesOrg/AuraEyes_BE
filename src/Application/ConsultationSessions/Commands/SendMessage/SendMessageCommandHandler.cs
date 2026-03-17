using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;

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

    public SendMessageCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IRepository<Conversation> conversationRepository,
        ICurrentUserService currentUser,
        IRepository<Ophthalmologist> ophthalmologistRepository,
        IRepository<Patient> patientRepository,
        INotificationService notificationService,
        IChatHubService chatHubService,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _conversationRepository = conversationRepository;
        _currentUser = currentUser;
        _ophthalmologistRepository = ophthalmologistRepository;
        _patientRepository = patientRepository;
        _notificationService = notificationService;
        _chatHubService = chatHubService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.ProfileId.HasValue)
            return Result.Unauthorized("Authenticated profile is required to send messages.");

        var senderProfileId = _currentUser.ProfileId.Value;

        var session = await _sessionRepository.GetByIdWithConversationsAsync(
            request.SessionId, cancellationToken);

        if (session is null)
            return Result.NotFound($"Session '{request.SessionId}' not found.");

        bool isPatient = senderProfileId == session.PatientId;
        bool isDoctor = session.OphthalmologistId.HasValue
                && senderProfileId == session.OphthalmologistId.Value;

        if (!isPatient && !isDoctor)
            return Result.Forbidden("You are not a participant of this session.");

        if (session.ChatStatus == ChatStatus.Locked)
            return Result.Failure("Chat is locked for this session.");

        if (session.ChatStatus == ChatStatus.Archived)
            return Result.Failure("Session has been archived. No new messages allowed.");

        if (session.ChatStatus == ChatStatus.MemoOnly
            && isDoctor)
        {
            return Result.Failure("In MemoOnly mode, only the patient can send notes.");
        }

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
        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Push realtime chat event to the other participant.
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

        if (recipientUserId.HasValue)
        {
            await _chatHubService.BroadcastChatMessageAsync(
                recipientUserId.Value,
                new ChatMessageRealtimeDto
                {
                    SessionId = session.Id,
                    MessageId = chatMessage.Id,
                    SenderProfileId = senderProfileId,
                    SentAt = chatMessage.SentAt
                },
                cancellationToken);
        }

        // Send real-time notification to the other party [FR-47]
        if (isPatient && session.OphthalmologistId.HasValue)
        {
            var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(
                session.OphthalmologistId.Value,
                cancellationToken);

            if (ophthalmologist is not null)
            {
                // Patient sent message -> Notify Doctor
                var messagePreview = request.Message.Length > 50
                    ? request.Message[..50] + "..."
                    : request.Message;

                await _notificationService.SendAsync(
                    ophthalmologist.UserId,
                    "Tin nhắn mới từ bệnh nhân",
                    $"Bạn có tin nhắn mới: \"{messagePreview}\"",
                    NotificationType.NewPatientMessage,
                    new { ConsultationId = session.Id, PatientId = session.PatientId },
                    cancellationToken);
            }
        }

        return Result.Success();
    }
}
