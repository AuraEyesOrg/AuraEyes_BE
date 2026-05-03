using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

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
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FinalizeMedicalRecordCommandHandler> _logger;

    public FinalizeMedicalRecordCommandHandler(
        IMedicalRecordRepository medicalRecordRepository,
        IPatientVisitRepository patientVisitRepository,
        IRepository<Domain.Entities.Users.Patient> patientRepository,
        IMedicalRecordPdfService pdfService,
        IFileStorageService fileStorageService,
        IEmailService emailService,
        INotificationService notificationService,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        ILogger<FinalizeMedicalRecordCommandHandler> logger)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _patientVisitRepository = patientVisitRepository;
        _patientRepository = patientRepository;
        _pdfService = pdfService;
        _fileStorageService = fileStorageService;
        _emailService = emailService;
        _notificationService = notificationService;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _logger = logger;
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

            // 1. Fetch Patient and Identity Data
            var patient = await _patientRepository.GetByIdAsync(record.PatientId, cancellationToken);
            if (patient == null)
            {
                return Result.NotFound("Patient profile not found.");
            }

            string? email = null;
            string? gender = null;

            if (patient.UserId.HasValue)
            {
                var userDetails = await _identityService.GetUserDetailsAsync(patient.UserId.Value, cancellationToken);
                if (userDetails != null)
                {
                    email = userDetails.Email;
                    gender = userDetails.Gender?.ToString();
                }
            }
            else
            {
                // Walk-in patient logic
                gender = patient.GenderId switch
                {
                    1 => "Male",
                    2 => "Female",
                    _ => "Other"
                };
                // Note: Walk-in patients might not have an email stored in the system yet.
                // If PhoneNumber is used as identifier, we might skip email.
            }

            var adminData = string.IsNullOrEmpty(record.AdministrativeDataJson)
                ? new Dictionary<string, object>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(record.AdministrativeDataJson);

            var name = adminData?.GetValueOrDefault("fullName")?.ToString();
            if (string.IsNullOrWhiteSpace(name)) name = patient.FullName;

            var dob = adminData?.GetValueOrDefault("birthDate")?.ToString();
            if (string.IsNullOrWhiteSpace(dob)) dob = patient.DateOfBirth?.ToString("dd/MM/yyyy");

            var addr = adminData?.GetValueOrDefault("address")?.ToString();
            if (string.IsNullOrWhiteSpace(addr)) addr = patient.Address;

            var gnd = adminData?.GetValueOrDefault("gender")?.ToString();
            if (string.IsNullOrWhiteSpace(gnd)) gnd = gender;

            // 2. Generate PDF
            var pdfModel = new MedicalRecordPdfModel
            {
                MedicalRecordNumber = record.MedicalRecordNumber,
                PatientName = name ?? "Unknown",
                DateOfBirth = dob,
                Gender = gnd,
                Address = addr,
                CreatedAt = record.CreatedAt,
                FinalDiagnosis = record.FinalDiagnosis,
                TreatmentPlan = record.TreatmentPlan,
                AdministrativeDataJson = record.AdministrativeDataJson,
                ClinicalDataJson = record.ClinicalDataJson
            };

            var pdfBytes = _pdfService.GenerateMedicalRecordPdf(pdfModel);

            // 3. Upload to Storage
            using var stream = new MemoryStream(pdfBytes);
            var fileName = $"EMR_{record.MedicalRecordNumber}.pdf";
            var relativePath = await _fileStorageService.SaveFileAsync(stream, fileName, $"medical-records/{record.PatientId}", cancellationToken);
            record.UpdatePdfUrl(relativePath);

            await _medicalRecordRepository.UpdateAsync(record, cancellationToken);

            // 4. Update PatientVisit Status
            if (record.PatientVisitId.HasValue)
            {
                var visit = await _patientVisitRepository.GetByIdAsync(record.PatientVisitId.Value, cancellationToken);
                if (visit != null && visit.Status == Domain.Enums.PatientVisitStatus.InProgress)
                {
                    visit.FinishConsultation("Hồ sơ bệnh án đã hoàn thành và khóa.");
                    await _patientVisitRepository.UpdateAsync(visit, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Send Email to Patient
            if (!string.IsNullOrEmpty(email))
            {
                var emailBody = $@"
                    <h3>Xin chào {patient.FullName},</h3>
                    <p>Hồ sơ bệnh án của bạn tại Aura Digital Clinic đã được hoàn thành.</p>
                    <p>Mã hồ sơ: <b>{record.MedicalRecordNumber}</b></p>
                    <p>Vui lòng xem file đính kèm để biết chi tiết chẩn đoán và hướng điều trị.</p>
                    <br/>
                    <p>Trân trọng,<br/>Đội ngũ Aura Digital Clinic</p>";

                await _emailService.SendWithAttachmentsAsync(
                    email,
                    "Hồ sơ bệnh án điện tử - Aura Digital Clinic",
                    emailBody,
                    new[] { new EmailAttachment(fileName, pdfBytes, "application/pdf") },
                    true,
                    cancellationToken);
            }

            // 6. Send Notifications
            if (patient.UserId.HasValue)
            {
                await _notificationService.SendAsync(
                    userId: patient.UserId.Value,
                    title: "Hồ sơ bệnh án đã hoàn thành",
                    message: $"Hồ sơ {record.MedicalRecordNumber} đã sẵn sàng. Bạn có thể xem trên ứng dụng hoặc email.",
                    type: NotificationType.ConsultationResultProvided, // Using existing type
                    payload: new { MedicalRecordId = record.Id, PdfUrl = relativePath },
                    cancellationToken: cancellationToken);
            }

            // Notify Clinic Staff (Coordinator)
            await _notificationService.SendToRoleAsync(
                roleName: Application.Common.Constants.Roles.ClinicStaff,
                title: "Medical Record Finalized",
                message: $"Patient {patient.FullName}'s medical record ({record.MedicalRecordNumber}) has been locked and archived.",
                type: NotificationType.SystemAlert,
                payload: new { MedicalRecordId = record.Id, VisitId = record.PatientVisitId, Action = "cashier_payment_ready" },
                cancellationToken: cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finalizing medical record {RecordId}", request.Id);
            return Result.Failure(ex.Message);
        }
    }
}
