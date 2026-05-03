using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.MedicalRecords;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.MedicalRecords.Queries.ExportMedicalRecordPdf;

public sealed class ExportMedicalRecordPdfQueryHandler
    : IQueryHandler<ExportMedicalRecordPdfQuery, MedicalRecordPdfFileDto>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMedicalRecordPdfService _medicalRecordPdfService;

    public ExportMedicalRecordPdfQueryHandler(
        IMedicalRecordRepository medicalRecordRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService,
        ICurrentUserService currentUserService,
        IMedicalRecordPdfService medicalRecordPdfService)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
        _currentUserService = currentUserService;
        _medicalRecordPdfService = medicalRecordPdfService;
    }

    public async Task<Result<MedicalRecordPdfFileDto>> Handle(
        ExportMedicalRecordPdfQuery request,
        CancellationToken cancellationToken)
    {
        var record = await _medicalRecordRepository.GetByIdAsNoTrackingAsync(request.MedicalRecordId, cancellationToken);
        if (record is null)
            return Result<MedicalRecordPdfFileDto>.NotFound("Medical record not found.");

        // Security Check: If user is a Patient, they can only see their own record
        if (_currentUserService.UserId.HasValue)
        {
            var patients = await _patientRepository.FindAsNoTrackingAsync(p => p.UserId == _currentUserService.UserId.Value, cancellationToken);
            var currentPatient = patients.FirstOrDefault();
            
            if (currentPatient != null && record.PatientId != currentPatient.Id && !_currentUserService.IsInRole("SystemAdmin") && !_currentUserService.IsInRole("Ophthalmologist") && !_currentUserService.IsInRole("ClinicStaff"))
            {
                return Result<MedicalRecordPdfFileDto>.Forbidden("You are not authorized to export this medical record.");
            }
        }

        if ((int)record.Status < (int)MedicalRecordStatus.Finalized)
            return Result<MedicalRecordPdfFileDto>.Failure("EMR must be finalized before downloading PDF.");

        var patient = await _patientRepository.GetByIdAsNoTrackingAsync(record.PatientId, cancellationToken);
        if (patient is null)
            return Result<MedicalRecordPdfFileDto>.NotFound("Patient profile not found.");

        string? gender = null;
        if (patient.UserId.HasValue)
        {
            var userDetails = await _identityService.GetUserDetailsAsync(patient.UserId.Value, cancellationToken);
            gender = userDetails?.Gender?.ToString();
        }

        gender ??= patient.GenderId switch
        {
            1 => "Male",
            2 => "Female",
            _ => "Other"
        };

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

        var pdfBytes = _medicalRecordPdfService.GenerateMedicalRecordPdf(new MedicalRecordPdfModel
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
        });

        return Result<MedicalRecordPdfFileDto>.Success(new MedicalRecordPdfFileDto
        {
            Content = pdfBytes,
            ContentType = "application/pdf",
            FileName = $"EMR_{record.MedicalRecordNumber}.pdf"
        });
    }
}

