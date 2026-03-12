using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Enums;
using Domain.Repositories;

namespace Application.ConsultationSessions.Commands.SendMessage;

public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public SendMessageCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IRepository<Conversation> conversationRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _conversationRepository = conversationRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdWithConversationsAsync(
            request.SessionId, cancellationToken);

        if (session is null)
            return Result.NotFound($"Session '{request.SessionId}' not found.");

        bool isPatient = request.SenderUserId == session.PatientId;
        bool isDoctor = session.OphthalmologistId.HasValue
                        && request.SenderUserId == session.OphthalmologistId.Value;

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

        var chatMessage = new ChatMessage(conversation.Id, request.SenderUserId, request.Message);
        conversation.AddMessage(chatMessage);

        session.RecordActivity();
        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send real-time notification to the other party [FR-47]
        if (isPatient && session.OphthalmologistId.HasValue)
        {
            // Patient sent message -> Notify Doctor
            var messagePreview = request.Message.Length > 50 
                ? request.Message[..50] + "..." 
                : request.Message;

            await _notificationService.SendAsync(
                session.OphthalmologistId.Value,
                "Tin nhắn mới từ bệnh nhân",
                $"Bạn có tin nhắn mới: \"{messagePreview}\"",
                NotificationType.NewPatientMessage,
                new { ConsultationId = session.Id, PatientId = session.PatientId },
                cancellationToken);
        }

        return Result.Success();
    }
}
