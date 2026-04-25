using Application.Common.Models;
using Domain.Repositories;
using MediatR;

namespace Application.MedicalRecords.Commands.FinalizeMedicalRecord;

public class FinalizeMedicalRecordCommandHandler : IRequestHandler<FinalizeMedicalRecordCommand, Result>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IRepository<Domain.Entities.Users.Patient> _patientRepository;
    private readonly IMedicalRecordPdfService _pdfService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public FinalizeMedicalRecordCommandHandler(
        IMedicalRecordRepository medicalRecordRepository,
        IPatientVisitRepository patientVisitRepository,
        IRepository<Domain.Entities.Users.Patient> patientRepository,
        IMedicalRecordPdfService pdfService,
        IFileStorageService fileStorageService,
        IEmailService emailService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _patientVisitRepository = patientVisitRepository;
        _patientRepository = patientRepository;
        _pdfService = pdfService;
        _fileStorageService = fileStorageService;
        _emailService = emailService;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(FinalizeMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _medicalRecordRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (record == null)
        {
            return Result.NotFound("Medical record not found.");
        }

        try
        {
            record.FinalizeRecord();

            // 1. Generate PDF
            var patient = await _patientRepository.GetByIdAsync(record.PatientId, cancellationToken);
            var pdfModel = new Application.Common.Interfaces.MedicalRecordPdfModel
            {
                MedicalRecordNumber = record.MedicalRecordNumber,
                PatientName = patient?.FullName ?? "Unknown",
                DateOfBirth = patient?.DateOfBirth?.ToString("dd/MM/yyyy"),
                Gender = patient?.Gender.ToString(),
                Address = patient?.Address,
                CreatedAt = record.CreatedAt,
                FinalDiagnosis = record.FinalDiagnosis,
                TreatmentPlan = record.TreatmentPlan,
                AdministrativeDataJson = record.AdministrativeDataJson,
                ClinicalDataJson = record.ClinicalDataJson
            };

            var pdfBytes = _pdfService.GenerateMedicalRecordPdf(pdfModel);

            // 2. Upload to Storage
            using var stream = new MemoryStream(pdfBytes);
            var fileName = $"EMR_{record.MedicalRecordNumber}.pdf";
            var relativePath = await _fileStorageService.SaveFileAsync(stream, fileName, $"medical-records/{record.PatientId}", cancellationToken);
            record.UpdatePdfUrl(relativePath);

            await _medicalRecordRepository.UpdateAsync(record, cancellationToken);

            // 3. Update PatientVisit Status
            if (record.PatientVisitId.HasValue)
            {
                var visit = await _patientVisitRepository.GetByIdAsync(record.PatientVisitId.Value, cancellationToken);
                if (visit != null)
                {
                    visit.Complete("Hồ sơ bệnh án đã hoàn thành và khóa.");
                    await _patientVisitRepository.UpdateAsync(visit, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 4. Send Email to Patient
            if (patient != null && !string.IsNullOrEmpty(patient.Email))
            {
                var emailBody = $@"
                    <h3>Xin chào {patient.FullName},</h3>
                    <p>Hồ sơ bệnh án của bạn tại Aura Digital Clinic đã được hoàn thành.</p>
                    <p>Mã hồ sơ: <b>{record.MedicalRecordNumber}</b></p>
                    <p>Vui lòng xem file đính kèm để biết chi tiết chẩn đoán và hướng điều trị.</p>
                    <br/>
                    <p>Trân trọng,<br/>Đội ngũ Aura Digital Clinic</p>";

                await _emailService.SendWithAttachmentsAsync(
                    patient.Email,
                    "Hồ sơ bệnh án điện tử - Aura Digital Clinic",
                    emailBody,
                    new[] { new Application.Common.Interfaces.EmailAttachment(fileName, pdfBytes, "application/pdf") },
                    true,
                    cancellationToken);
            }

            // 5. Send Notifications
            // Notify Patient
            await _notificationService.SendToUserAsync(
                userId: record.PatientId,
                title: "Hồ sơ bệnh án đã hoàn thành",
                message: $"Hồ sơ {record.MedicalRecordNumber} đã sẵn sàng. Bạn có thể xem trên ứng dụng hoặc email.",
                type: Application.Common.Interfaces.NotificationType.HealthUpdate,
                payload: new { MedicalRecordId = record.Id, PdfUrl = relativePath },
                cancellationToken: cancellationToken);

            // Notify Clinic Staff (Coordinator)
            await _notificationService.SendToRoleAsync(
                roleName: Application.Common.Constants.Roles.ClinicStaff,
                title: "Medical Record Finalized",
                message: $"Patient {patient?.FullName}'s medical record ({record.MedicalRecordNumber}) has been locked and archived.",
                type: Application.Common.Interfaces.NotificationType.SystemAlert,
                payload: new { MedicalRecordId = record.Id, VisitId = record.PatientVisitId },
                cancellationToken: cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
