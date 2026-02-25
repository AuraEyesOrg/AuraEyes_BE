using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ConsultationSessionService : IConsultationSessionService
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IRepository<MedicalDiagnosis> _diagnosisRepository;
    private readonly ApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ConsultationSessionService> _logger;

    public ConsultationSessionService(
        IConsultationSessionRepository sessionRepository,
        IRepository<Conversation> conversationRepository,
        IRepository<MedicalDiagnosis> diagnosisRepository,
        ApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ILogger<ConsultationSessionService> logger)
    {
        _sessionRepository = sessionRepository;
        _conversationRepository = conversationRepository;
        _diagnosisRepository = diagnosisRepository;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> CreateVerificationSessionAsync(
        Guid patientId,
        Guid aiScreeningId,
        decimal price,
        Guid? ophthalmologistId = null,
        CancellationToken cancellationToken = default)
    {
        var session = ConsultationSession.CreateVerification(
            patientId, aiScreeningId, price, ophthalmologistId);

        await _sessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Verification session {SessionId} created for patient {PatientId}",
            session.Id, patientId);

        return Result<Guid>.Success(session.Id);
    }

    public async Task<Result<Guid>> CreateVideoCallSessionAsync(
        Guid patientId,
        decimal price,
        DateTime appointmentTime,
        Guid? ophthalmologistId = null,
        string? meetingLink = null,
        CancellationToken cancellationToken = default)
    {
        var session = ConsultationSession.CreateVideoCall(
            patientId, price, appointmentTime, ophthalmologistId, meetingLink);

        await _sessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "VideoCall session {SessionId} created for patient {PatientId}, appointment at {AppointmentTime}",
            session.Id, patientId, appointmentTime);

        return Result<Guid>.Success(session.Id);
    }

    public async Task<Result> SubmitVerificationReportAsync(
        Guid sessionId,
        Guid doctorId,
        string diagnosesCode,
        string diagnosesText,
        string? treatmentPlan = null,
        CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
        if (session is null)
            return Result.NotFound($"Session {sessionId} not found");

        if (session.Type != ConsultationSessionType.Verification)
            return Result.Failure("Only verification sessions accept reports");

        if (session.OphthalmologistId.HasValue && session.OphthalmologistId.Value != doctorId)
            return Result.Forbidden("You are not assigned to this session");

        if (!session.AiScreeningId.HasValue)
            return Result.Failure("Session has no linked AI screening");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var diagnosis = new MedicalDiagnosis(
                session.AiScreeningId.Value, doctorId, diagnosesCode, diagnosesText);

            if (treatmentPlan is not null)
                diagnosis.UpdateDiagnosis(diagnosesCode, diagnosesText, treatmentPlan);

            await _diagnosisRepository.AddAsync(diagnosis, cancellationToken);

            session.OpenChat();
            await _sessionRepository.UpdateAsync(session, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Verification report submitted for session {SessionId} by doctor {DoctorId}. Chat is now Open.",
                sessionId, doctorId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Failed to submit verification report for session {SessionId}", sessionId);
            return Result.Failure("An error occurred while submitting the report");
        }
    }

    public async Task<Result> SendMessageAsync(
        Guid sessionId,
        Guid senderUserId,
        string message,
        CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdWithConversationsAsync(sessionId, cancellationToken);
        if (session is null)
            return Result.NotFound($"Session {sessionId} not found");

        if (session.ChatStatus == ChatStatus.Locked)
            return Result.Failure("Chat is locked for this session");

        if (session.ChatStatus == ChatStatus.Archived)
            return Result.Failure("Session has been archived. No new messages allowed.");

        if (session.ChatStatus == ChatStatus.MemoOnly && session.OphthalmologistId.HasValue
            && senderUserId == session.OphthalmologistId.Value)
            return Result.Failure("In MemoOnly mode, only the patient can send notes");

        var conversation = session.Conversations.FirstOrDefault();
        if (conversation is null)
        {
            if (!session.OphthalmologistId.HasValue)
                return Result.Failure("No ophthalmologist assigned to this session yet");

            conversation = new Conversation(session.OphthalmologistId.Value, sessionId);
            await _conversationRepository.AddAsync(conversation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var chatMessage = new ChatMessage(conversation.Id, senderUserId, message);
        await _dbContext.ChatMessages.AddAsync(chatMessage, cancellationToken);

        session.RecordActivity();
        await _sessionRepository.UpdateAsync(session, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> EndSessionAsync(
        Guid sessionId,
        Guid doctorId,
        CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
        if (session is null)
            return Result.NotFound($"Session {sessionId} not found");

        if (session.Status == SessionStatus.Completed)
            return Result.Failure("Session is already completed");

        try
        {
            session.EndSession(doctorId);
            await _sessionRepository.UpdateAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Session {SessionId} ended by doctor {DoctorId}",
                sessionId, doctorId);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Forbidden(ex.Message);
        }
    }
}
