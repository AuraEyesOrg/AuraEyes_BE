using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Helpers;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ConsultationSessions.Commands.CreateVideoCallSession;

public class CreateVideoCallSessionCommandHandler
    : ICommandHandler<CreateVideoCallSessionCommand, Guid>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateVideoCallSessionCommandHandler> _logger;

    public CreateVideoCallSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IRepository<Patient> patientRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateVideoCallSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _patientRepository = patientRepository;
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

        var normalizedAppointmentTimeUtc = NormalizeAppointmentTimeToUtc(request.AppointmentTime);

        var session = ConsultationSession.CreateVideoCall(
            patientId: request.PatientId,
            price: request.Price,
            appointmentTime: normalizedAppointmentTimeUtc,
            ophthalmologistId: request.OphthalmologistId);

        await _sessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "VideoCall session {SessionId} created for patient {PatientId}",
            session.Id, request.PatientId);

        return Result<Guid>.Success(session.Id);
    }

    private static DateTime NormalizeAppointmentTimeToUtc(DateTime appointmentTime)
    {
        if (appointmentTime.Kind == DateTimeKind.Utc)
        {
            return appointmentTime;
        }

        if (appointmentTime.Kind == DateTimeKind.Local)
        {
            return appointmentTime.ToUniversalTime();
        }

        return TimeZoneInfo.ConvertTimeToUtc(appointmentTime, VietnamTimeZoneResolver.TimeZone);
    }
}