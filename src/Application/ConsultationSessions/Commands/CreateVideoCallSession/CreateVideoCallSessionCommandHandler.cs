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
    private readonly IRepository<Patient> _patientRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;
    private readonly IGoogleMeetService _googleMeetService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateVideoCallSessionCommandHandler> _logger;

    public CreateVideoCallSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IRepository<Patient> patientRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService,
        IGoogleMeetService googleMeetService,
        IUnitOfWork unitOfWork,
        ILogger<CreateVideoCallSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _patientRepository = patientRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
        _googleMeetService = googleMeetService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateVideoCallSessionCommand request,
        CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Result<Guid>.NotFound($"Patient '{request.PatientId}' not found.");

        var attendeeEmails = await ResolveAttendeeEmailsAsync(
            patient, request.OphthalmologistId, cancellationToken);

        MeetingInfo meetingInfo;
        try
        {
            meetingInfo = await _googleMeetService.CreateMeetingAsync(
                "AURA Consultation",
                request.AppointmentTime,
                attendeeEmails,
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
            meetingInfo.MeetingLink,
            meetingInfo.CalendarEventId);

        await _sessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "VideoCall session {SessionId} created with Meet link for patient {PatientId}, attendees: [{Attendees}]",
            session.Id, request.PatientId, string.Join(", ", attendeeEmails));

        return Result<Guid>.Success(session.Id);
    }

    private async Task<List<string>> ResolveAttendeeEmailsAsync(
        Patient patient,
        Guid? ophthalmologistId,
        CancellationToken cancellationToken)
    {
        var emails = new List<string>();

        var patientUser = await _identityService.GetUserByIdAsync(patient.UserId, cancellationToken);
        if (patientUser is not null)
            emails.Add(patientUser.Email);

        if (ophthalmologistId.HasValue)
        {
            var doctor = await _ophthalmologistRepository.GetByIdAsync(
                ophthalmologistId.Value, cancellationToken);

            if (doctor is not null)
            {
                var doctorUser = await _identityService.GetUserByIdAsync(
                    doctor.UserId, cancellationToken);

                if (doctorUser is not null)
                    emails.Add(doctorUser.Email);
            }
        }

        return emails;
    }
}
