using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ConsultationSessions.Commands.CreateVideoCallSession;

public class CreateVideoCallSessionCommandHandler
    : ICommandHandler<CreateVideoCallSessionCommand, Guid>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IGoogleMeetService _googleMeetService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateVideoCallSessionCommandHandler> _logger;

    public CreateVideoCallSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IGoogleMeetService googleMeetService,
        IUnitOfWork unitOfWork,
        ILogger<CreateVideoCallSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _googleMeetService = googleMeetService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateVideoCallSessionCommand request,
        CancellationToken cancellationToken)
    {
        MeetingInfo meetingInfo;
        try
        {
            meetingInfo = await _googleMeetService.CreateMeetingAsync(
                $"AURA Consultation – Patient {request.PatientId}",
                request.AppointmentTime,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Google Meet link for patient {PatientId}", request.PatientId);
            return Result<Guid>.Failure("Unable to generate video call link. Please try again later.");
        }

        var session = ConsultationSession.CreateVideoCall(
            request.PatientId,
            request.Price,
            request.AppointmentTime,
            request.OphthalmologistId,
            meetingInfo.MeetingLink);

        await _sessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "VideoCall session {SessionId} created with Meet link for patient {PatientId}",
            session.Id, request.PatientId);

        return Result<Guid>.Success(session.Id);
    }
}
