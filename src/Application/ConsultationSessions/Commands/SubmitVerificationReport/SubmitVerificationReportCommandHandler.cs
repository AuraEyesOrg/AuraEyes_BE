using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;

namespace Application.ConsultationSessions.Commands.SubmitVerificationReport;

public class SubmitVerificationReportCommandHandler
    : ICommandHandler<SubmitVerificationReportCommand>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<MedicalDiagnosis> _diagnosisRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitVerificationReportCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IRepository<MedicalDiagnosis> diagnosisRepository,
        IRepository<Patient> patientRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _diagnosisRepository = diagnosisRepository;
        _patientRepository = patientRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        SubmitVerificationReportCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
            return Result.NotFound($"Session '{request.SessionId}' not found.");

        if (session.Type != ConsultationSessionType.Verification)
            return Result.Failure("Only verification sessions accept reports.");

        if (session.OphthalmologistId.HasValue && session.OphthalmologistId.Value != request.DoctorId)
            return Result.Forbidden("You are not assigned to this session.");

        if (!session.AiScreeningId.HasValue)
            return Result.Failure("Session has no linked AI screening.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var diagnosis = new MedicalDiagnosis(
                    session.AiScreeningId.Value,
                    request.DoctorId,
                    session.Id,
                    request.DiagnosesCode,
                    request.DiagnosesText,
                    request.TreatmentPlan);

            await _diagnosisRepository.AddAsync(diagnosis, cancellationToken);

            session.OpenChat();
            await _sessionRepository.UpdateAsync(session, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var patient = await _patientRepository.GetByIdAsync(session.PatientId, cancellationToken);
            if (patient is not null)
            {
                // Send real-time notification to Patient [FR-46]
                await _notificationService.SendAsync(
                    patient.UserId,
                    "Kết quả tư vấn đã sẵn sàng",
                    "Bác sĩ đã hoàn tất báo cáo xác minh kết quả sàng lọc của bạn. Bạn có thể xem chi tiết và trao đổi trực tiếp với bác sĩ.",
                    NotificationType.ConsultationResultProvided,
                    new { ConsultationId = session.Id, DoctorId = request.DoctorId },
                    cancellationToken);
            }

            return Result.Success();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
